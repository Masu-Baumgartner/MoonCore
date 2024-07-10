using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Attributes;
using MoonCore.Models;
using MoonCore.Services;

namespace MoonCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// This adds the mooncore identity system
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="configuration">Make changes to the configuration in this action</param>
    public static void AddMoonCoreIdentity(this IServiceCollection collection, Action<MoonCoreIdentityConfiguration>? configuration)
    {
        MoonCoreIdentityConfiguration config = new();
        
        if(configuration != null)
            configuration.Invoke(config);

        collection.AddSingleton(config);

        collection.AddScoped<IdentityService>();
    }
}