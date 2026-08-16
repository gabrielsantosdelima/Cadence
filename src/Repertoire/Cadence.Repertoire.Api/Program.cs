using Cadence.Repertoire.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RepertoireDbContext>(options => options.UseSqlite("Data Source=repertoire.db"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
