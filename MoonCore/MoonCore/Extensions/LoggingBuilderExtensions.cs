using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MoonCore.Extensions;

public static class LoggingBuilderExtensions
{
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

    public static void AddProviders(this ILoggingBuilder builder, ILoggerProvider[] providers)
    {
        foreach (var provider in providers)
            builder.AddProvider(provider);
    }
}