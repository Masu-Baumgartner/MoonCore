using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.PluginFramework.Extensions;
using MoonCore.Test.Interfaces;

// Logging
var providers = LoggerBuildHelper.BuildFromConfiguration(configuration =>
{
    configuration.Console.Enable = true;
    configuration.Console.EnableAnsiMode = true;
    configuration.FileLogging.Enable = false;
});

var startupLoggerFactory = new LoggerFactory();
startupLoggerFactory.AddProviders(providers);

var startupLogger = startupLoggerFactory.CreateLogger("Startup");

// Startup DI
var startupServiceCollection = new ServiceCollection();

startupServiceCollection.AddLogging(builder =>
{
    builder.AddProviders(providers);
});

startupServiceCollection.AddPlugins(configuration =>
{
    configuration.AddInterface<IAppStartup>();
    
    configuration.AddAssembly(Assembly.GetEntryAssembly()!);
}, startupLogger);

var startupServiceProvider = startupServiceCollection.BuildServiceProvider();

var startupStuff = startupServiceProvider.GetRequiredService<IAppStartup[]>();

foreach (var appStartup in startupStuff)
    appStartup.BuildWebApplication();

foreach (var appStartup in startupStuff)
    appStartup.ConfigureWebApplication();

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder =>
{
    builder.AddProviders(providers);
});

serviceCollection.AddPlugins(configuration =>
{
    
    
    configuration.AddAssembly(Assembly.GetEntryAssembly()!);
}, startupLogger);

var serviceProvider = serviceCollection.BuildServiceProvider();

await Task.Delay(-1);