using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.SingleDb;
using MoonCore.Extended.Test;

var serviceCollection = new ServiceCollection();

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

var scope = serviceProvider.CreateScope();

var dbc = scope.ServiceProvider.GetRequiredService<DataContext>();

var pending = await dbc.Database.GetPendingMigrationsAsync();

if (pending.Any())
    await dbc.Database.MigrateAsync();

