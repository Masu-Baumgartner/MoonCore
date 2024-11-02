using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.DiscordNet.Extensions;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.Test;

// Logging
var providers = LoggerBuildHelper.BuildFromConfiguration(configuration =>
{
    configuration.Console.Enable = true;
    configuration.Console.EnableAnsiMode = true;
    configuration.FileLogging.Enable = false;
});

var startupLoggerFactory = new LoggerFactory();
startupLoggerFactory.AddProviders(providers);

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton<CoolSharedService>();

serviceCollection.AddDiscordBot(configuration =>
{
    configuration.ModuleAssemblies.Add(Assembly.GetEntryAssembly()!);
});

serviceCollection.AddLogging(builder =>
{
    builder.AddProviders(providers);
});

var serviceProvider = serviceCollection.BuildServiceProvider();

await Task.Delay(-1);