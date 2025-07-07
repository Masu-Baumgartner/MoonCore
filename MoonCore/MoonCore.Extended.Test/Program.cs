using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Extensions;
using MoonCore.Extended.SingleDb;
using MoonCore.Extended.Test;
using MoonCore.Extensions;
using MoonCore.Logging;

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder =>
{
    builder.AddAnsiConsole();
});

serviceCollection.AddDatabaseMappings();
serviceCollection.AddServiceCollectionAccessor();

serviceCollection.AddSingleton(new DatabaseOptions()
{
    Host = "localhost",
    Port = 5432,
    Username = "test_db",
    Database = "test_db",
    Password = "test_db"
});

serviceCollection.AddDbContext<DataContext>();

var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.EnsureDatabaseMigrated();
serviceProvider.GenerateDatabaseMappings();

var scope = serviceProvider.CreateScope();

var dc = scope.ServiceProvider.GetRequiredService<DataContext>();

Console.WriteLine(await dc.Database.CanConnectAsync());

Console.WriteLine("Exit");

