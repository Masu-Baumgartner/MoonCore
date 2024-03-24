using Microsoft.Extensions.Configuration;
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

    /// <summary>
    /// Adds a logging configuration similar to the appsettings.json but using a string
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configAsJson"></param>
    public static void AddConfiguration(this ILoggingBuilder builder, string configAsJson)
    {
        var config = new ConfigurationBuilder().AddJsonString(configAsJson);

        builder.AddConfiguration(config.Build());
    }
}