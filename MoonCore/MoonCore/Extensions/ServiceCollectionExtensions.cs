using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoonCore.Attributes;
using MoonCore.Configuration;
using MoonCore.Services;

namespace MoonCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// This checks all types in the assembly of the provided type for mooncore attributes to add them to the di
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    public static void AutoAddServices<T>(this IServiceCollection collection)
    {
        var assembly = typeof(T).Assembly;
        
        collection.AutoAddServices(assembly);
    }
    
    /// <summary>
    /// This checks all types in the assembly of the provided type for mooncore attributes to add them to the di
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    public static void AutoAddServices(this IServiceCollection collection, Assembly assembly)
    {
        var ownAssembly = typeof(ServiceCollectionExtensions).Assembly;

        foreach (var type in assembly.GetTypes())
        {
            var attributes = type
                .GetCustomAttributes(true)
                .Where(x => x.GetType().Assembly == ownAssembly)
                .ToArray();

            if (attributes.Any(x => x.GetType() == typeof(TransientAttribute)))
            {
                collection.AddTransient(type);
                continue;
            }
            
            if (attributes.Any(x => x.GetType() == typeof(ScopedAttribute)))
            {
                collection.AddScoped(type);
                continue;
            }
            
            if (attributes.Any(x => x.GetType() == typeof(SingletonAttribute)))
            {
                collection.AddSingleton(type);
                continue;
            }
            
            if (attributes.Any(x => x.GetType() == typeof(HostedServiceAttribute)))
            {
                collection.AddSingleton<IHostedService>(provider => (IHostedService)provider.GetRequiredService(type));
                collection.AddSingleton(type);
                continue;
            }
        }
    }

    public static void AddConfiguration(this IServiceCollection collection, Action<ConfigurationOptions> onConfigure)
    {
        var options = new ConfigurationOptions();
        onConfigure.Invoke(options);

        var configurationService = collection.FindRegisteredInstance<ConfigurationService>();

        if (configurationService == null)
        {
            configurationService = new ConfigurationService();
            collection.AddSingleton(configurationService);
        }
        
        configurationService.RegisterInDi(options, collection);
    }

    public static T? FindRegisteredInstance<T>(this IServiceCollection collection) where T : class
    {
        var serviceType = typeof(T);
        var descriptor = collection.FirstOrDefault(x => x.ServiceType == serviceType);

        if (descriptor == null)
            return default;

        return descriptor.ImplementationInstance as T;
    }
}