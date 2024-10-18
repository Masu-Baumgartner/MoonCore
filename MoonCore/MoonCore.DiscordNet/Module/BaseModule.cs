

using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.DiscordNet.configuration;
using MoonCore.PluginFramework.Abstractions;

namespace MoonCore.DiscordNet.Module;

public abstract class BaseModule : BasePlugin
{
    protected DiscordSocketClient Client;
    protected DiscordBotConfiguration Configuration { get; set; }
    protected IServiceProvider Provider;

    public BaseModule(ILogger logger) : base(logger) {}

    public void Init(DiscordSocketClient client, DiscordBotConfiguration configuration, IServiceProvider provider)
    {
        Client = client;
        Configuration = configuration;
        Provider = provider;
    }

    /// <summary>
    /// This is an async implementation to Load EventHandlers
    /// </summary>
    public abstract Task InitializeAsync();

    /// <summary>
    /// This is an async implementation to Unload EventHandlers
    /// </summary>
    public abstract Task UnloadAsync();
    
    /// <summary>
    /// This is an async implementation to Registering for Example SlashCommands
    /// </summary>
    public abstract Task RegisterAsync();

    
}