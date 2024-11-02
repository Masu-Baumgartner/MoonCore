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
}