using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Helpers;
using MoonCore.Helpers;

namespace MoonCore.Extended.Extensions;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Ensures all registered DbContext instances have all of their migrations run
    /// </summary>
    /// <param name="serviceProvider">Service provider to retrieve the db contexts</param>
    public static async Task EnsureDatabaseMigratedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        // Load all registered db context implementation...
        var contextTypes = scope.ServiceProvider.GetDbContexts();
        
        // ... to resolve them from the di, in order to access the migration actions
        var databaseContexts = contextTypes.Select(
            contextType => (scope.ServiceProvider.GetRequiredService(contextType) as DbContext)!
        ).ToArray();

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Database Migration");

        logger.LogInformation("Checking {count} database contexts for pending migrations", databaseContexts.Length);

        foreach (var databaseContext in databaseContexts)
        {
            var pendingMigrations = await databaseContext.Database.GetPendingMigrationsAsync();
            var migrationNames = pendingMigrations.ToArray();

            // Skip database without any pending migrations
            if (migrationNames.Length == 0)
                continue;

            logger.LogInformation("Pending migrations for '{name}': {migrations}",
                databaseContext.GetType().FullName,
                string.Join(", ", migrationNames)
            );

            logger.LogInformation("Migrating...");

            await databaseContext.Database.MigrateAsync();

            logger.LogInformation("Migration done!");
        }
    }

    /// <summary>
    /// Generates the DbContext - Entity mappings used by the <see cref="DatabaseRepository{TEntity}"/>
    /// </summary>
    /// <param name="serviceProvider">Service provider to retrieve the DbContexts from</param>
    /// <exception cref="ArgumentException">Throws if you forgot to call the <see cref="Extensions.ServiceCollectionExtensions.AddDatabaseMappings"/> method</exception>
    public static void GenerateDatabaseMappings(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var contextTypes = scope.ServiceProvider.GetDbContexts();

        var mappingOptions = scope.ServiceProvider.GetService<DatabaseMappingOptions>();

        if (mappingOptions == null)
            throw new ArgumentException(
                "You need to call AddDatabaseMappings() while registering services before generating the mappings");

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Database Mappings");

        logger.LogInformation("Generating mappings for {count} database contexts", contextTypes.Length);

        mappingOptions.Mappings.Clear();

        foreach (var contextType in contextTypes)
        {
            logger.LogInformation("Generating database mappings for {name}", contextType.FullName);

            foreach (var property in contextType.GetProperties())
            {
                var propType = property.PropertyType;

                if (!propType.IsGenericType)
                    continue;

                if (!ReflectionHelper.IsGenericVersionOf(propType, typeof(DbSet<>)))
                    continue;

                mappingOptions.Mappings[propType.GetGenericArguments().First()] = contextType;
            }
        }

        logger.LogInformation("Generated {count} entity-context mappings", mappingOptions.Mappings.Count);
    }

    /// <summary>
    /// Retrieves all DbContexts from a di service provider
    /// </summary>
    /// <param name="serviceProvider">Service provider to retrieve the DbContexts from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Throws if you forgot to call <see cref="Extensions.ServiceCollectionExtensions.AddServiceCollectionAccessor"/></exception>
    public static Type[] GetDbContexts(this IServiceProvider serviceProvider)
    {
        var scAccessor = serviceProvider.GetService<ServiceCollectionAccessor>();

        if (scAccessor == null)
            throw new ArgumentException(
                "You need to call AddServiceCollectionAccessor() on the service collection in order to use this method");

        var dbContextType = typeof(DbContext);

        return scAccessor.ServiceCollection
            .Where(x => x.ServiceType.IsAssignableTo(dbContextType) && x.ServiceType != dbContextType)
            .Select(x => x.ServiceType)
            .ToArray();
    }
}