using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extensions;
using MoonCore.PluginFramework.Configuration;
using MoonCore.PluginFramework.Services;

namespace MoonCore.PluginFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInterfaces(this IServiceCollection collection, Action<InterfaceConfiguration> onConfigure)
    {
        var configuration = new InterfaceConfiguration();
        onConfigure.Invoke(configuration);

        // Construct service

        var interfaceService = collection.FindRegisteredInstance<InterfaceService>();

        if (interfaceService == null)
        {
            interfaceService = new InterfaceService();
            collection.AddSingleton(interfaceService);
        }
        
        interfaceService.RegisterInterfaces(configuration, collection);
    }
}