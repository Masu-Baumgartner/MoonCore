using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.DiscordNet.Services;

namespace MoonCore.DiscordNet.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task StartDiscordBot(this IServiceProvider provider, bool runAsync = false, Assembly[]? moduleAssemblies = null)
    {
        var discordBotService = provider.GetRequiredService<DiscordBotService>();

        // If no assemblies have been specified we are using the entry assembly
        if (moduleAssemblies == null)
            moduleAssemblies = [Assembly.GetEntryAssembly()!];
        
        await discordBotService.Configure(moduleAssemblies);

        if (runAsync)
            Task.Run(discordBotService.StartAsync);
        else
            await discordBotService.StartAsync();
    }
}