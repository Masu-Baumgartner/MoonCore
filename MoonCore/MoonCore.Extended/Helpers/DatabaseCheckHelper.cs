using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MoonCore.Helpers;

public static class DatabaseCheckHelper
{
    public static async Task Check(ILogger<DbContext> logger, DbContext dbContext, bool enableConnectCheck = true)
    {
        logger.LogInformation("Checking '{name}' (Provider: {providerName})", dbContext.GetType().Name, dbContext.Database.ProviderName);
        var canConnect = await dbContext.Database.CanConnectAsync();

        if (!canConnect && !dbContext.Database.IsSqlite() && enableConnectCheck)
        {
            logger.LogCritical("Unable to connect to the database");
            throw new Exception("Unable to connect to database");
        }

        logger.LogInformation("Loading pending migrations");
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync())
            .ToArray();

        if (!pendingMigrations.Any())
        {
            logger.LogInformation("Database is up-to-date");
            return;
        }
        
        //TODO: Auto backup?
        
        logger.LogInformation("There are {pendingMigrations} migrations pending:", pendingMigrations.Length);

        foreach (var migration in pendingMigrations)
        {
            logger.LogInformation($"- {migration}");
        }
        
        logger.LogInformation("Applying migrations...");
        await dbContext.Database.MigrateAsync();
        
        logger.LogInformation("Database is up-to-date");
    }
}