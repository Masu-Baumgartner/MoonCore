using Microsoft.Extensions.Logging;
using MoonCore.Logging;

var loggerFactory = new LoggerFactory();

loggerFactory.AddAnsiConsole();

var logger = loggerFactory.CreateLogger<Program>();

try
{
    var y = 69;
    var x = 0;

    var a = y / x;
}
catch (Exception e)
{
    logger.LogError(e, "An exception occurred");
}