using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Abstractions;
using MoonCore.Attributes;
using MoonCore.Helpers;

namespace MoonCore.Extensions;

public static class ServiceProviderExtensions
{
    public static void StartBackgroundServices<T>(this IServiceProvider provider)
    {
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
                    Logger.Fatal($"A background service ({backgroundServiceType.Name}) has crashed:");
                    Logger.Fatal(e);
                }
            });
        }
    }
    
    public static void StopBackgroundServices<T>(this IServiceProvider provider)
    {
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
                    Logger.Fatal($"A background service ({backgroundServiceType.Name}) has crashed:");
                    Logger.Fatal(e);
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