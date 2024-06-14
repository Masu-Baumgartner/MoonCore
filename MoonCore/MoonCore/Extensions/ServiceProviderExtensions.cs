using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Abstractions;
using MoonCore.Attributes;
using MoonCore.Helpers;

namespace MoonCore.Extensions;

public static class ServiceProviderExtensions
{
    public static void StartBackgroundServices<T>(this IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<BackgroundService>>();
        
        var assembly = typeof(T).Assembly;

        foreach (var backgroundServiceType in GetBackgroundServiceTypes(assembly))
        {
            var backGroundService = provider.GetRequiredService(backgroundServiceType) as BackgroundService;

            Task.Run(async () =>
            {
                try
                {
                    await backGroundService!.Run();
                }
                catch (Exception e)
                {
                    logger.LogCritical("A background service ({name}) has crashed: {e}", backgroundServiceType.Name, e);
                }
            });
        }
    }
    
    public static void StopBackgroundServices<T>(this IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<BackgroundService>>();
        
        var assembly = typeof(T).Assembly;

        foreach (var backgroundServiceType in GetBackgroundServiceTypes(assembly))
        {
            var backGroundService = provider.GetRequiredService(backgroundServiceType) as BackgroundService;

            Task.Run(async () =>
            {
                try
                {
                    await backGroundService!.Stop();
                }
                catch (Exception e)
                {
                    logger.LogCritical("A background service ({name}) has crashed: {e}", backgroundServiceType.Name, e);
                }
            });
        }
    }

    private static Type[] GetBackgroundServiceTypes(Assembly assembly)
    {
        var ownAssembly = typeof(ServiceCollectionExtensions).Assembly;

        return assembly
            .GetTypes()
            .Where(x => x.GetCustomAttributes()
                .Where(y => y.GetType().Assembly == ownAssembly)
                .Any(y => y.GetType() == typeof(BackgroundServiceAttribute)))
            .ToArray();
    }
}