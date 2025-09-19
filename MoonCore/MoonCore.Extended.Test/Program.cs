using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Extensions;
using MoonCore.Extended.Test;
using MoonCore.Logging;

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder =>
{
    builder.AddAnsiConsole();
});

serviceCollection.AddDatabaseMappings();
serviceCollection.AddServiceCollectionAccessor();

serviceCollection.AddDbContext<DataContext>();

var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.EnsureDatabaseMigratedAsync();
serviceProvider.GenerateDatabaseMappings();

var scope = serviceProvider.CreateScope();

var dc = scope.ServiceProvider.GetRequiredService<DataContext>();

Console.WriteLine(await dc.Database.CanConnectAsync());

Console.WriteLine("Exit");

