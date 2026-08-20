using Cadence.Repertoire.Api.Consumers;
using Cadence.Repertoire.Api.Endpoints;
using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";
string frontendOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";
string rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://admin:admin@localhost:5672";

builder.Services.AddDbContext<RepertoireDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Repertoire") ?? "Data Source=repertoire.db"));

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

using (IServiceScope migrationScope = app.Services.CreateScope())
{
    migrationScope.ServiceProvider.GetRequiredService<RepertoireDbContext>().Database.Migrate();
}

app.UseCors(FrontendCorsPolicy);

app.MapPieceEndpoints();
app.MapHealthChecks("/health");

app.Run();
