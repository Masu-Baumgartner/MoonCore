using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoonCore.Extended.Abstractions;
using MoonCore.Extended.Extensions;
using MoonCore.Extended.Helpers;
using MoonCore.Helpers;

namespace MoonCore.Extended.Services;

/// <summary>
/// Hosted service which automatically generates the database mappings from the registered <see cref="DbContext" /> instances
/// so the <see cref="DatabaseRepository{TEntity}"/> can resolve the entities
/// </summary>
public class DatabaseMappingsService : IHostedLifecycleService
{
    private readonly ILogger<DatabaseMappingsService> Logger;
    private readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Hosted service which automatically generates the database mappings from the registered <see cref="DbContext" /> instances
    /// so the <see cref="DatabaseRepository{TEntity}"/> can resolve the entities
    /// </summary>
    /// <param name="logger">Logger to use for reporting on the progress</param>
    /// <param name="serviceProvider">Service provider to resolve the db context instances</param>
    public DatabaseMappingsService(ILogger<DatabaseMappingsService> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Task StartingAsync(CancellationToken cancellationToken)
    {
        using var scope = ServiceProvider.CreateScope();

        var contextTypes = scope.ServiceProvider.GetDbContexts();

        var mappingOptions = scope.ServiceProvider.GetService<DatabaseMappingOptions>();

        if (mappingOptions == null)
        {
            throw new ArgumentException("You need to call AddDatabaseMappings() while registering services before generating the mappings");
        }
        
        Logger.LogInformation("Generating mappings for {count} database contexts", contextTypes.Length);

        foreach (var contextType in contextTypes)
        {
            Logger.LogInformation("Generating database mappings for {name}", contextType.FullName);

            foreach (var property in contextType.GetProperties())
            {
                var propType = property.PropertyType;

                if (!propType.IsGenericType)
                    continue;

                if (!ReflectionHelper.IsGenericVersionOf(propType, typeof(DbSet<>)))
                    continue;

                mappingOptions.Mappings[propType.GetGenericArguments()[0]] = contextType;
            }
        }

        Logger.LogInformation("Generated {count} entity-context mappings", mappingOptions.Mappings.Count);
        return Task.CompletedTask;
    }

    #region Unused

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task StartedAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task StoppedAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task StoppingAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    #endregion
}