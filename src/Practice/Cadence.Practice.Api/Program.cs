using Cadence.Practice.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PracticeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Practice") ?? "Data Source=practice.db"));

var app = builder.Build();

using (IServiceScope migrationScope = app.Services.CreateScope())
{
    migrationScope.ServiceProvider.GetRequiredService<PracticeDbContext>().Database.Migrate();
}

app.MapGet("/", () => "Hello World!");

app.Run();
