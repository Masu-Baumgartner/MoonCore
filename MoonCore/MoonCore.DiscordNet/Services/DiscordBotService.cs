using System.Reflection;
using Discord;
using Microsoft.Extensions.Logging;
using Discord.WebSocket;
using MoonCore.DiscordNet.configuration;
using MoonCore.DiscordNet.Extensions;
using MoonCore.DiscordNet.Module;
using MoonCore.PluginFramework.Services;

namespace MoonCore.DiscordNet.Services;

public class DiscordBotService
{
    private ILogger<DiscordBotService> Logger { get; set; }
    private readonly DiscordSocketClient Client;
    private readonly IServiceProvider Provider;
    private readonly PluginService<BaseModule> ModuleService;
    private DiscordBotConfiguration Configuration;
    
    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        IServiceProvider provider,
        PluginService<BaseModule> moduleService,
        DiscordBotConfiguration configuration,
        DiscordSocketConfig socketConfig)
    {
        Logger = logger;
        Provider = provider;
        ModuleService = moduleService;
        Configuration = configuration;

        Client = new(socketConfig);
    }
    
    public async Task Configure(Assembly[] moduleAssemblies)
    {
        // Register modules
        await ModuleService.Load(moduleAssemblies);
        
        // Initialize modules
        await ModuleService.Call(async x => x.Init(Client, Configuration, Provider));
    }

    public async Task StartAsync()
    {
        Client.Log += Log;
        Client.Ready += OnReady;
        
        //AddModules modules to the SocketClient
        await ModuleService.Call(x => x.InitializeAsync());
        
        await Client.LoginAsync(TokenType.Bot, Configuration.Auth.Token);
        await Client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task OnReady()
    {
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetGameAsync("the Universe", "https://spielepapagei.de", ActivityType.Listening);

        if(Configuration.Settings.DevelopMode)
            Logger.LogInformation("Invite link: {invite}", $"https://discord.com/api/oauth2/authorize?client_id={Client.CurrentUser.Id}&permissions=1099511696391&scope=bot%20applications.commands");
        
        Logger.LogInformation("Login as {username}#{id}", Client.CurrentUser.Username, Client.CurrentUser.DiscriminatorValue);

        //Initialize SlashCommands and Stuff
        await ModuleService.Call(x => x.RegisterAsync());
    }

    private Task Log(LogMessage message)
    {
        message.ToILogger(Logger);
        return Task.CompletedTask;
    }
    
}