using Discord;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.DiscordNet.Extensions;
using MoonCore.Extensions;

// Basic config
var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder =>
{
    builder.AddMoonCore(configuration =>
    {
        configuration.FileLogging.Enable = false;
        configuration.Console.Enable = true;
    }); 
});

serviceCollection.AddDiscordBot(configuration =>
{
    configuration.Settings.DevelopMode = true;
    configuration.Settings.EnableDebug = true;
    configuration.Settings.Enable = true;

    configuration.Auth.Token = "owo";
    configuration.Auth.TokenType = TokenType.Bot;
}, socketConfig =>
{
    socketConfig.GatewayIntents = GatewayIntents.All;
});

var provider = serviceCollection.BuildServiceProvider();

await provider.StartDiscordBot();