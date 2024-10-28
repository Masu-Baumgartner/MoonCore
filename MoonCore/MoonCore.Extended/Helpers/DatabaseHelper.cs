using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Helpers;

namespace MoonCore.Extended.Helpers;

public class DatabaseHelper
{
    private readonly ILogger<DatabaseHelper> Logger;

    private readonly List<Type> DbContextTypes = new();
    private readonly Dictionary<Type, Type> Mappings = new();

    public DatabaseHelper(ILogger<DatabaseHelper> logger)
    {
        Logger = logger;
    }

    public void AddDbContext<T>() where T : DbContext
        => DbContextTypes.Add(typeof(T));
    
    public void AddDbContext(Type type)
        => DbContextTypes.Add(type);
    
    public Type? GetDbContextForEntity<T>() where T : class
    {
        if (Mappings.TryGetValue(typeof(T), out var result))
            return result;

        return null;
    }

    public void GenerateMappings()
    {
        Mappings.Clear();

        foreach (var contextType in DbContextTypes)
        {
            Logger.LogInformation("Generating database mappings for {name}", contextType.FullName);

            foreach (var property in contextType.GetProperties())
            {
                var propType = property.PropertyType;

                if (!propType.IsGenericType)
                    continue;

                if (!ReflectionHelper.IsGenericVersionOf(propType, typeof(DbSet<>)))
                    continue;

                Mappings[propType.GetGenericArguments().First()] = contextType;
            }
        }

        Logger.LogInformation("Generated {count} entity-context mappings", Mappings.Count);
    }

    public async Task EnsureMigrated(IServiceProvider provider)
    {
        foreach (var contextType in DbContextTypes)
        {
            Logger.LogInformation("Looking for pending migrations in {name}", contextType.FullName);

            var context = (DbContext)provider.GetRequiredService(contextType);

            var pendingMigrations = (await context.Database.GetPendingMigrationsAsync())
                .ToArray();

            if (pendingMigrations.Length == 0)
            {
                Logger.LogInformation("Database is up-to-date");
                continue;
            }

            Logger.LogInformation("There are {pendingMigrations} migrations pending: {migrations}",
                pendingMigrations.Length,
                string.Join(" ", pendingMigrations)
            );
            
            Logger.LogInformation("Migrating...");
            
            await context.Database.MigrateAsync();
            
            Logger.LogInformation("Migrated successfully");
        }
    }
}