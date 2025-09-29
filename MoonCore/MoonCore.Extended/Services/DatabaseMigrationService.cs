using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoonCore.Extended.Extensions;

namespace MoonCore.Extended.Services;

/// <summary>
/// Hosted service which automatically performs all pending migrations of all registered <see cref="DbContext"/>
/// </summary>
public class DatabaseMigrationService : IHostedLifecycleService
{
    private readonly ILogger<DatabaseMigrationService> Logger;
    private readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Hosted service which automatically performs all pending migrations of all registered <see cref="DbContext"/>
    /// </summary>
    /// <param name="logger">Logger used to log the progress of the migration</param>
    /// <param name="serviceProvider">Service provider to create scope and retrieve db context instances</param>
    public DatabaseMigrationService(
        ILogger<DatabaseMigrationService> logger,
        IServiceProvider serviceProvider
    )
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Automatically performing database migrations on registered DbContexts");
        
        await using var scope = ServiceProvider.CreateAsyncScope();

        // Load all registered db context implementation...
        var contextTypes = scope.ServiceProvider.GetDbContexts();
        
        // ... to resolve them from the di, in order to access the migration actions
        var databaseContexts = contextTypes.Select(
            contextType => (scope.ServiceProvider.GetRequiredService(contextType) as DbContext)!
        ).ToArray();

        Logger.LogInformation("Checking {count} database contexts for pending migrations", databaseContexts.Length);

        foreach (var databaseContext in databaseContexts)
        {
            var pendingMigrations = await databaseContext.Database.GetPendingMigrationsAsync(cancellationToken);
            var migrationNames = pendingMigrations.ToArray();

            // Skip database without any pending migrations
            if (migrationNames.Length == 0)
                continue;

            Logger.LogInformation("Pending migrations for '{name}': {migrations}",
                databaseContext.GetType().FullName,
                string.Join(", ", migrationNames)
            );

            Logger.LogInformation("Migrating...");

            await databaseContext.Database.MigrateAsync(cancellationToken);

            Logger.LogInformation("Migration done!");
        }
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