using Microsoft.Extensions.Logging;
using MoonCore.Configuration;
using MoonCore.Helpers;

namespace MoonCore.Extensions;

public static class LoggerFactoryExtensions
{
    public static void AddProviders(this ILoggerFactory factory, ILoggerProvider[] providers)
    {
        foreach (var provider in providers)
            factory.AddProvider(provider);
    }

    public static void AddMoonCore(this ILoggerFactory factory, Action<LoggingConfiguration> onConfigure)
    {
        factory.AddProviders(LoggerBuildHelper.BuildFromConfiguration(onConfigure));
    }
    
    public static void AddMoonCore(this ILoggerFactory factory)
    {
        factory.AddProviders(LoggerBuildHelper.BuildFromConfiguration(configuration =>
        {
            configuration.Console.Enable = true;
            configuration.Console.EnableAnsiMode = true;
            configuration.FileLogging.Enable = false;
        }));
    }
}