using Microsoft.Extensions.Logging;
using MoonCore.Extensions;

var loggerFactory = new LoggerFactory();
loggerFactory.AddMoonCore();

var logger = loggerFactory.CreateLogger("Test");

logger.LogInformation("Testy");