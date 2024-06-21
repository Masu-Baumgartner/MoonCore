using Microsoft.Extensions.Logging;
using MoonCore.Logging;
using MoonCore.Models;

namespace MoonCore.Helpers;

public static class LoggerBuildHelper
{
    public static ILoggerProvider[] BuildFromConfiguration(MoonCoreLoggingConfiguration configuration)
    {
        var result = new List<ILoggerProvider>();
        
        if (configuration.Console.Enable)
            result.Add(new ConsoleLoggingProvider(configuration.Console.EnableAnsiMode));

        if (configuration.FileLogging.Enable)
        {
            string? GetCurrentRotateName()
            {
                var counter = 0;
                string currentName;

                while (true)
                {
                    if (string.IsNullOrEmpty(configuration.FileLogging.RotateLogNameTemplate))
                        currentName = $"{configuration.FileLogging.Path}.{counter}";
                    else
                        currentName = string.Format(configuration.FileLogging.RotateLogNameTemplate, counter);

                    if (!File.Exists(currentName))
                        break;

                    counter++;
                }

                return currentName;
            }

            if (File.Exists(configuration.FileLogging.Path))
            {
                var currentRotateName = GetCurrentRotateName();
                    
                if(currentRotateName != null)
                    File.Move(configuration.FileLogging.Path, currentRotateName);
            }

            result.Add(new FileLoggingProvider(configuration.FileLogging.Path));
        }

        return result.ToArray();
    }
}