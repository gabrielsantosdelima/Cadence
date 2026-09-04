using Cadence.Repertoire.Api.Consumers;
using Cadence.Repertoire.Api.Endpoints;
using Cadence.Repertoire.Api.Middleware;
using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Infrastructure;
using Cadence.Repertoire.Infrastructure.Migrations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

builder.Services.AddDbContextPool<RepertoireDbContext>(options =>
    ConfigureRepertoireDatabase(options, builder.Configuration));

builder.Services.AddScoped<IPieceRepository, PieceRepository>();
builder.Services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<RepertoireDbContext>());
builder.Services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.AddConsumer<PracticeSessionRegisteredConsumer>();

    busConfigurator.UsingRabbitMq((context, rabbitMqConfigurator) =>
    {
        rabbitMqConfigurator.Host(new Uri(rabbitMqConnectionString));

        rabbitMqConfigurator.ReceiveEndpoint("practice-session-registered", receiveEndpointConfigurator =>
        {
            receiveEndpointConfigurator.UseMessageRetry(retryConfigurator =>
                retryConfigurator.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));

            receiveEndpointConfigurator.ConfigureConsumer<PracticeSessionRegisteredConsumer>(context);
        });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<RepertoireDbContext>("database")
    .AddRabbitMQ(async _ =>
    {
        ConnectionFactory connectionFactory = new() { Uri = new Uri(rabbitMqConnectionString) };
        return await connectionFactory.CreateConnectionAsync();
    }, name: "rabbitmq");

var app = builder.Build();

if (applyMigrationsOnStartup)
{
    using IServiceScope migrationScope = app.Services.CreateScope();
    migrationScope.ServiceProvider.GetRequiredService<RepertoireDbContext>().Database.Migrate();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseCors(FrontendCorsPolicy);

app.MapPieceEndpoints();
app.MapHealthChecks("/health");

app.Run();
return 0;

static void ConfigureRepertoireDatabase(DbContextOptionsBuilder options, IConfiguration configuration)
{
    string provider = configuration["Database:Provider"] ?? "Sqlite";

    if (string.Equals(provider, "Postgres", StringComparison.OrdinalIgnoreCase))
    {
        NpgsqlConnectionStringBuilder connectionStringBuilder = new(
            configuration.GetConnectionString("RepertoirePostgres")
                ?? "Host=localhost;Port=5432;Database=cadence_repertoire;Username=cadence;Password=cadence")
        {
            MaxPoolSize = configuration.GetValue("Database:Postgres:MaxPoolSize", 20),
            MinPoolSize = configuration.GetValue("Database:Postgres:MinPoolSize", 2),
            Timeout = configuration.GetValue("Database:Postgres:TimeoutSeconds", 5)
        };

        options.UseNpgsql(connectionStringBuilder.ConnectionString);
    }
    else
    {
        options.UseSqlite(configuration.GetConnectionString("Repertoire") ?? "Data Source=repertoire.db");
    }

    options.ReplaceService<IMigrationsAssembly, ProviderFilteredMigrationsAssembly>();

    if (configuration.GetValue<bool>("Database:LogSql"))
        options.LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information);
}

static async Task<int> RunMigrationsAsync(string[] args)
{
    HostApplicationBuilder migrateBuilder = Host.CreateApplicationBuilder(args);

    migrateBuilder.Services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
    migrateBuilder.Services.AddDbContext<RepertoireDbContext>(options =>
        ConfigureRepertoireDatabase(options, migrateBuilder.Configuration));

    using IHost migrateHost = migrateBuilder.Build();
    using IServiceScope migrateScope = migrateHost.Services.CreateScope();
    RepertoireDbContext dbContext = migrateScope.ServiceProvider.GetRequiredService<RepertoireDbContext>();

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
