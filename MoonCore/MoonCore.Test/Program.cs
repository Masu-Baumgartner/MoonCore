using System.Reflection;
using MoonCore.Attributes;
using MoonCore.Startup;
using MoonCore.Test;

var startupService = new StartupService<IStartupLayer>();

Console.WriteLine("Preparing");
startupService.Prepare([Assembly.GetEntryAssembly()!]);

await startupService.Run(layer => layer.Run());