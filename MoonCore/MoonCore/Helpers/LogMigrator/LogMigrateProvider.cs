using Microsoft.Extensions.Logging;

namespace MoonCore.Helpers.LogMigrator;

public class LogMigrateProvider : ILoggerProvider
{
    public void Dispose() {}

    public ILogger CreateLogger(string categoryName)
    {
        return new MigrateLogger();
    }
}