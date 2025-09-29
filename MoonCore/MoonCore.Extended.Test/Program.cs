using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoonCore.Extended.Extensions;
using MoonCore.Extended.Test;
using MoonCore.Logging;

var builder = Host.CreateApplicationBuilder();

var serviceCollection = new ServiceCollection();

builder.Logging.ClearProviders();
builder.Logging.AddAnsiConsole();

builder.Services.AddDatabaseMappings();
builder.Services.AddDbAutoMigrations();

builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

var scope = app.Services.CreateScope();

var dc = scope.ServiceProvider.GetRequiredService<DataContext>();

Console.WriteLine(await dc.Database.CanConnectAsync());

await app.RunAsync();

Console.WriteLine("Exit");

