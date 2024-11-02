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
    private readonly IBaseModule[] Modules;
    private readonly DiscordBotConfiguration Configuration;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        DiscordBotConfiguration configuration,
        IBaseModule[] modules,
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

        foreach (var module in Modules)
            await module.InitializeAsync();

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

        foreach (var module in Modules)
            await module.RegisterAsync();
    }

    private Task Log(LogMessage message)
    {
        message.ToILogger(Logger);
        return Task.CompletedTask;
    }
}