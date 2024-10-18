using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.DiscordNet.configuration;
using MoonCore.DiscordNet.Services;
using MoonCore.PluginFramework.Services;

namespace MoonCore.DiscordNet.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDiscordBot(this IServiceCollection collection, Action<DiscordBotConfiguration> onConfigure, Action<DiscordSocketConfig>? onConfigureSocket = null)
    {
        // Bot config
        var configuration = new DiscordBotConfiguration();
        onConfigure.Invoke(configuration);

        collection.AddSingleton(configuration);
        
        // Socket config
        var socketConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };
        
        onConfigureSocket?.Invoke(socketConfig);

        collection.AddSingleton(socketConfig);
        
        // Main service
        collection.AddSingleton<DiscordBotService>();

        // Register plugin service if none has been registered yet
        if (collection.All(x => x.ImplementationType != typeof(PluginService<>)))
            collection.AddSingleton(typeof(PluginService<>));
    }
}