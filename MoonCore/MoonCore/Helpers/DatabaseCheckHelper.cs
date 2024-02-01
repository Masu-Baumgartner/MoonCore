using Microsoft.EntityFrameworkCore;

namespace MoonCore.Helpers;

public static class DatabaseCheckHelper
{
    public static async Task Check(DbContext dbContext, bool enableConnectCheck = true)
    {
        Logger.Info($"Checking '{dbContext.GetType().Name}' (Provider: {dbContext.Database.ProviderName})");
        
        var canConnect = await dbContext.Database.CanConnectAsync();

        if (!canConnect && !dbContext.Database.IsSqlite() && enableConnectCheck)
        {
            Logger.Fatal("Unable to connect to the database");
            throw new Exception("Unable to connect to database");
        }

        Logger.Info("Loading pending migrations");
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync())
            .ToArray();

        if (!pendingMigrations.Any())
        {
            Logger.Info("Database is up-to-date");
            return;
        }
        
        //TODO: Auto backup?
        
        Logger.Info($"There are {pendingMigrations.Length} migrations pending:");

        foreach (var migration in pendingMigrations)
        {
            Logger.Info($"- {migration}");
        }
        
        Logger.Info("Applying migrations...");
        await dbContext.Database.MigrateAsync();
        
        Logger.Info("Database is up-to-date");
    }
}