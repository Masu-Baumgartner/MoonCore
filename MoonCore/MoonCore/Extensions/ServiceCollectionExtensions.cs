﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Attributes;
using MoonCore.Models;
using MoonCore.Services;

namespace MoonCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// This does nothing at the moment
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="configuration">Make changes to the configuration in this action</param>
    public static void AddMoonCore(this IServiceCollection collection, Action<MoonCoreConfiguration>? configuration)
    {
        MoonCoreConfiguration config = new();
        
        if(configuration != null)
            configuration.Invoke(config);

        collection.AddSingleton(config);

        collection.AddScoped<IdentityService>();
    }

    /// <summary>
    /// This checks all types in the assembly of the provided type for mooncore attributes to add them to the di
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    public static void ConstructMoonCoreDi<T>(this IServiceCollection collection)
    {
        var assembly = typeof(T).Assembly;
        
        collection.ConstructMoonCoreDi(assembly);
    }
    
    /// <summary>
    /// This checks all types in the assembly of the provided type for mooncore attributes to add them to the di
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    public static void ConstructMoonCoreDi(this IServiceCollection collection, Assembly assembly)
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
            
            if (attributes.Any(x => x.GetType() == typeof(BackgroundServiceAttribute)))
            {
                collection.AddSingleton(type);
                continue;
            }
        }
    }
}