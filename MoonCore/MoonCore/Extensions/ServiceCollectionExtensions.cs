using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Attributes;

namespace MoonCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// This checks all types in the assembly of the provided type for mooncore attributes to add them to the di
    /// </summary>
    /// <param name="collection">Service collection to add the services to</param>
    /// <typeparam name="T">Assembly marker for the assembly to search through</typeparam>
    public static void AutoAddServices<T>(this IServiceCollection collection)
    {
        var assembly = typeof(T).Assembly;
        
        collection.AutoAddServices(assembly);
    }

    /// <summary>
    /// This checks all types in the assembly of the provided type for mooncore attributes to add them to the di
    /// </summary>
    /// <param name="collection">Service collection to add the services to</param>
    /// <param name="assembly">Assembly to search through</param>
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
        }
    }
}