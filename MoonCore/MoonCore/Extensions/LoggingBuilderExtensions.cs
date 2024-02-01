using Microsoft.Extensions.Logging;
using MoonCore.Helpers.LogMigrator;

namespace MoonCore.Extensions;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// This migrates every log message from microsoft's logging system to moon cores logging
    /// </summary>
    /// <param name="builder"></param>
    public static void MigrateToMoonCore(this ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.AddProvider(new LogMigrateProvider());
    }
}