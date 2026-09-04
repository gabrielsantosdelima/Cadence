using Cadence.Practice.Api.Endpoints;
using Cadence.Practice.Api.Messaging;
using Cadence.Practice.Api.Middleware;
using Cadence.Practice.Domain;
using Cadence.Practice.Infrastructure;
using Cadence.Practice.Infrastructure.Migrations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using RabbitMQ.Client;

if (args.Contains("--migrate"))
{
    return await RunMigrationsAsync(args);
}

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";
string frontendOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";
string rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://admin:admin@localhost:5672";
bool applyMigrationsOnStartup = builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

builder.Services.AddDbContextPool<PracticeDbContext>(options =>
    ConfigurePracticeDatabase(options, builder.Configuration));

builder.Services.AddScoped<ISessionRepository, SessionRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(typeof(CorrelationIdPublishFilter<>));

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.UsingRabbitMq((context, rabbitMqConfigurator) =>
    {
        rabbitMqConfigurator.Host(new Uri(rabbitMqConnectionString));
        rabbitMqConfigurator.UsePublishFilter(typeof(CorrelationIdPublishFilter<>), context);
        rabbitMqConfigurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PracticeDbContext>("database")
    .AddRabbitMQ(async _ =>
    {
        ConnectionFactory connectionFactory = new() { Uri = new Uri(rabbitMqConnectionString) };
        return await connectionFactory.CreateConnectionAsync();
    }, name: "rabbitmq");

var app = builder.Build();

if (applyMigrationsOnStartup)
{
    using IServiceScope migrationScope = app.Services.CreateScope();
    migrationScope.ServiceProvider.GetRequiredService<PracticeDbContext>().Database.Migrate();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseCors(FrontendCorsPolicy);

app.MapSessionEndpoints();
app.MapHealthChecks("/health");

app.Run();
return 0;

static void ConfigurePracticeDatabase(DbContextOptionsBuilder options, IConfiguration configuration)
{
    string provider = configuration["Database:Provider"] ?? "Sqlite";

    if (string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder = new(
            configuration.GetConnectionString("PracticePostgres")
                ?? "Host=localhost;Port=5432;Database=cadence_practice;Username=cadence;Password=cadence")
        {
            MaxPoolSize = configuration.GetValue("Database:Postgres:MaxPoolSize", 20),
            MinPoolSize = configuration.GetValue("Database:Postgres:MinPoolSize", 2),
            Timeout = configuration.GetValue("Database:Postgres:TimeoutSeconds", 5)
        };

        options.UseNpgsql(connectionStringBuilder.ConnectionString);
    }
    else
    {
        options.UseSqlite(configuration.GetConnectionString("Practice") ?? "Data Source=practice.db");
    }

    options.ReplaceService<IMigrationsAssembly, ProviderFilteredMigrationsAssembly>();

    if (configuration.GetValue<bool>("Database:LogSql"))
        options.LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information);
}

static async Task<int> RunMigrationsAsync(string[] args)
{
    HostApplicationBuilder migrateBuilder = Host.CreateApplicationBuilder(args);

    migrateBuilder.Services.AddDbContext<PracticeDbContext>(options =>
        ConfigurePracticeDatabase(options, migrateBuilder.Configuration));

    using IHost migrateHost = migrateBuilder.Build();
    using IServiceScope migrateScope = migrateHost.Services.CreateScope();
    PracticeDbContext dbContext = migrateScope.ServiceProvider.GetRequiredService<PracticeDbContext>();

    List<string> pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();

    if (pendingMigrations.Count == 0)
    {
        Console.WriteLine("No pending migrations.");
        return 0;
    }

    await dbContext.Database.MigrateAsync();
    Console.WriteLine($"Applied {pendingMigrations.Count} migration(s): {string.Join(", ", pendingMigrations)}");
    return 0;
}
