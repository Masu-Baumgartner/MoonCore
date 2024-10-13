using System.Data;
using Discord;
using Microsoft.Extensions.Logging;
using Discord.WebSocket;
using MoonCore.DiscordNet.configuration;
using MoonCore.DiscordNet.Module;
using MoonCore.PluginFramework.Services;

namespace MoonCore.DiscordNet.Services;

public class DiscordAppService
{
    private ILogger<DiscordAppService> Logger { get; set; }
    private readonly DiscordSocketClient Client;
    private readonly IServiceProvider Provider;
    private readonly PluginService<BaseModule> ModuleService;
    private MoonCoreDiscordAppConfiguration Configuration;


    public DiscordAppService(
        ILogger<DiscordAppService> logger,
        IServiceProvider provider,
        PluginService<BaseModule> moduleService,
        DiscordSocketConfig discordSocketConfig)
    {
        Logger = logger;
        Provider = provider;
        ModuleService = moduleService;

        if (Configuration == null) throw new NoNullAllowedException();
        Client = new DiscordSocketClient(discordSocketConfig);
    }
    
    public async void Configure(MoonCoreDiscordAppConfiguration configuration)
    {
        Configuration = configuration;
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
        {
            Logger.LogInformation($"Invite link: https://discord.com/api/oauth2/authorize?client_id={Client.CurrentUser.Id}&permissions=1099511696391&scope=bot%20applications.commands");
        }
        Logger.LogInformation($"Login as {Client.CurrentUser.Username}#{Client.CurrentUser.DiscriminatorValue}");

        //Initialize SlashCommands and Stuff
        //await ModuleService.Call(x => x.RegisterAsync());
    }

    private Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                Logger.LogCritical(message.Message);
                return Task.CompletedTask;
            case LogSeverity.Error:
                Logger.LogError(message.Message);
                return Task.CompletedTask;
            case LogSeverity.Warning:
                Logger.LogWarning(message.Message);
                return Task.CompletedTask;
            case LogSeverity.Info:
                Logger.LogInformation(message.Message);
                return Task.CompletedTask;
            case LogSeverity.Verbose:
                Logger.LogDebug(message.Message);
                return Task.CompletedTask;
            case LogSeverity.Debug:
                if (Configuration.Settings.EnableDebug) 
                { Logger.LogDebug(message.Message); }
                return Task.CompletedTask;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}