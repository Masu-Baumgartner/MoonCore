

using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.DiscordNet.configuration;
using MoonCore.PluginFramework.Abstractions;

namespace MoonCore.DiscordNet.Module;

public abstract class BaseModule : BasePlugin
{
    private DiscordSocketClient Client;
    private MoonCoreDiscordAppConfiguration Configuration { get; set; }
    private IServiceProvider Provider;

    protected BaseModule(ILogger<BasePlugin> logger) : base(logger)
    {}

    public void Init(DiscordSocketClient client, MoonCoreDiscordAppConfiguration configuration, IServiceProvider provider)
    {
        Client = client;
        Configuration = configuration;
        Provider = provider;
    }

    /// <summary>
    /// This is a async implementation to Load EventHandlers
    /// </summary>
    public abstract Task InitializeAsync();

    /// <summary>
    /// This is a async implementation to Unload EventHandlers
    /// </summary>
    public abstract Task UnloadAsync();
    
    /// <summary>
    /// This is a async implementation to Registering for Example SlashCommands
    /// </summary>
    public abstract Task RegisterAsync();

    
}