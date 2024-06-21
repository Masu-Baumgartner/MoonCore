using Microsoft.Extensions.Logging;

namespace MoonCore.Extensions;

public static class LoggerFactoryExtensions
{
    public static void AddProviders(this ILoggerFactory factory, ILoggerProvider[] providers)
    {
        foreach (var provider in providers)
            factory.AddProvider(provider);
    }
}