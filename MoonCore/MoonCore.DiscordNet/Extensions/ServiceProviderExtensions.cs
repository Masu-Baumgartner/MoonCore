using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.DiscordNet.Services;

namespace MoonCore.DiscordNet.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task StartDiscordBot(this IServiceProvider provider, bool runAsync = false)
    {
        var discordBotService = provider.GetRequiredService<DiscordBotService>();

        if (runAsync)
            Task.Run(discordBotService.StartAsync);
        else
            await discordBotService.StartAsync();
    }
}