using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MoonCore.DiscordNet.Module;

namespace MoonCore.Test.Modules;

public class MyCoolModule : IBaseBotModule
{
    private readonly DiscordSocketClient Client;
    private readonly CoolSharedService CoolSharedService;
    private readonly ILogger<MyCoolModule> Logger;

    public MyCoolModule(DiscordSocketClient client, CoolSharedService coolSharedService, ILogger<MyCoolModule> logger)
    {
        Client = client;
        CoolSharedService = coolSharedService;
        Logger = logger;
    }

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task UnloadAsync()
        => Task.CompletedTask;

    public Task RegisterAsync()
        => Task.CompletedTask;
}