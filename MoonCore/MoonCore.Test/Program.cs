using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.Test.Configuration;

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

var serviceCollection = new ServiceCollection();

serviceCollection.AddConfiguration(options =>
{
    options.AddConfiguration<AConfig>();
});

serviceCollection.AddLogging(builder =>
{
    builder.AddProviders(providers);
});

var serviceProvider = serviceCollection.BuildServiceProvider();

var configA = serviceProvider.GetRequiredService<AConfig>();

startupLogger.LogInformation("A > Value1: {val}", configA.Value1);
startupLogger.LogInformation("A > Value2: {val}", configA.Value2);

await Task.Delay(-1);