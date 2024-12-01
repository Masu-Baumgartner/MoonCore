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
    private readonly ILogger<DiscordBotService> Logger;
    private readonly DiscordSocketClient Client;
    private readonly IBaseBotModule[] Modules;
    private readonly DiscordBotConfiguration Configuration;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        DiscordBotConfiguration configuration,
        IBaseBotModule[] modules,
        DiscordSocketClient client)
    {
        Logger = logger;
        Modules = modules;
        Client = client;
        Configuration = configuration;
    }

    public async Task StartAsync()
    {
        Client.Log += Log;
        Client.Ready += OnReady;

        try
        {
            foreach (var module in Modules)
                await module.InitializeAsync();
        }
        catch (NotImplementedException)
        {
            Logger.LogDebug("not implemented yet.");
        }
        catch (Exception e)
        {
            Logger.LogError("An error occurred during Module initialization: {RegisterException}", e);
        }

        await Client.LoginAsync(TokenType.Bot, Configuration.Auth.Token);
        await Client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task OnReady()
    {
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetGameAsync("the Universe", "https://spielepapagei.de", ActivityType.Listening);

        if (Configuration.Settings.DevelopMode)
            Logger.LogInformation("Invite link: {invite}",
                $"https://discord.com/api/oauth2/authorize?client_id={Client.CurrentUser.Id}&permissions=1099511696391&scope=bot%20applications.commands");

        Logger.LogInformation("Login as {username}#{id}", Client.CurrentUser.Username,
            Client.CurrentUser.DiscriminatorValue);

        try
        {
            foreach (var module in Modules)
                await module.RegisterAsync();
        }
        catch (NotImplementedException) {}
        catch (Exception e)
        {
            Logger.LogError("An error occurred during Module registration: {RegisterException}", e);
        }
        
    }

    public IBaseBotModule[] GetBaseBotModules()
    {
        return Modules;
    }

    public async Task UnregisterAsync(IBaseBotModule module)
    {
        await module.UnregisterAsync();
    }

    private Task Log(LogMessage message)
    {
        message.ToILogger(Logger);
        return Task.CompletedTask;
    }
}