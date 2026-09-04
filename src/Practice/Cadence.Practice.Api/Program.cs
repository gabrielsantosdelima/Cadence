using Cadence.Practice.Api.Endpoints;
using Cadence.Practice.Api.Messaging;
using Cadence.Practice.Api.Middleware;
using Cadence.Practice.Domain;
using Cadence.Practice.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "Frontend";
string frontendOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";
string rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq") ?? "amqp://admin:admin@localhost:5672";

builder.Services.AddDbContext<PracticeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Practice") ?? "Data Source=practice.db"));

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

using (IServiceScope migrationScope = app.Services.CreateScope())
{
    migrationScope.ServiceProvider.GetRequiredService<PracticeDbContext>().Database.Migrate();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseCors(FrontendCorsPolicy);

app.MapSessionEndpoints();
app.MapHealthChecks("/health");

app.Run();
