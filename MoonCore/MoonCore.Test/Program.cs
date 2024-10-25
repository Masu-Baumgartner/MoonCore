using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.PluginFramework.Extensions;
using MoonCore.Test.Implementations;
using MoonCore.Test.Interfaces;

var providers = LoggerBuildHelper.BuildFromConfiguration(configuration =>
{
    configuration.Console.Enable = true;
    configuration.Console.EnableAnsiMode = true;
    configuration.FileLogging.Enable = false;
});

var startupLoggerFactory = new LoggerFactory();
startupLoggerFactory.AddProviders(providers);

var startupLogger = startupLoggerFactory.CreateLogger("Startup");

var serviceCollection = new ServiceCollection();

serviceCollection.AddLogging(builder =>
{
    builder.AddProviders(providers);
});

serviceCollection.AddPlugins(configuration =>
{
    configuration.AddInterface<IExample>();
    
    configuration.AddAssembly(Assembly.GetEntryAssembly()!);
}, startupLogger);

serviceCollection.AddSingleton<Example2>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var examples = serviceProvider.GetRequiredService<IExample[]>();

startupLogger.LogDebug("Calling plugins");
foreach (var example in examples)
    example.DoSmth();

startupLogger.LogDebug("Res: {res}", serviceProvider.GetService<Example2>() != null);

await Task.Delay(-1);