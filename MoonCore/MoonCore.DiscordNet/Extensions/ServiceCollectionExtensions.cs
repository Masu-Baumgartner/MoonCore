using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.DiscordNet.configuration;
using MoonCore.DiscordNet.Module;
using MoonCore.DiscordNet.Services;
using MoonCore.PluginFramework.Extensions;
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
        
        // Discord socket client
        var discordSocketClient = new DiscordSocketClient(socketConfig);
        collection.AddSingleton(discordSocketClient);
        
        // Main service
        collection.AddSingleton<DiscordBotService>();

        //
        collection.AddPlugins(interfaceConfiguration =>
        {
            interfaceConfiguration.AddAssemblies(configuration.ModuleAssemblies);
            interfaceConfiguration.AddInterface<IBaseBotModule>();
        });
    }
}