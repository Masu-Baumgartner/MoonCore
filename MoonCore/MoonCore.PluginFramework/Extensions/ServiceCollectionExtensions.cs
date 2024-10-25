using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.PluginFramework.Configuration;
using MoonCore.PluginFramework.Services;

namespace MoonCore.PluginFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPlugins(this IServiceCollection collection, Action<InterfaceConfiguration> onConfigure, ILogger logger)
    {
        var configuration = new InterfaceConfiguration();
        onConfigure.Invoke(configuration);

        // Construct service
        var pluginService = new InterfaceService(configuration, logger);
        pluginService.ConstructInterfaces(collection);
        
        collection.AddSingleton(pluginService);
    }
}