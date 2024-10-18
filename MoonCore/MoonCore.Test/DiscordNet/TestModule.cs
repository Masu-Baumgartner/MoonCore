using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.DiscordNet.configuration;
using MoonCore.DiscordNet.Module;
using MoonCore.PluginFramework.Abstractions;

namespace MoonCore.Test.DiscordNet;

public class TestModule : BaseModule
{
    public TestModule(ILogger logger) : base(logger)
    {
    }

    public override Task InitializeAsync()
    {
        Client.MessageReceived += OnMessage;
        return Task.CompletedTask;
    }

    private async Task OnMessage(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        if (!message.Content.Contains("ping")) return;
        
        await message.Channel.SendMessageAsync("Pong");
    }

    public override Task UnloadAsync()
    {
        Client.MessageReceived -= OnMessage;
        return Task.CompletedTask;
    }

    public override Task RegisterAsync()
    {
        throw new NotImplementedException();
    }
}