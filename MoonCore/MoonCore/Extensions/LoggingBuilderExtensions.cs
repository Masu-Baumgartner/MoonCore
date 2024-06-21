using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoonCore.Logging;
using MoonCore.Models;

namespace MoonCore.Extensions;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds a logging configuration similar to the appsettings.json but using a string
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configAsJson"></param>
    public static void AddConfiguration(this ILoggingBuilder builder, string configAsJson)
    {
        var config = new ConfigurationBuilder().AddJsonString(configAsJson);

        builder.AddConfiguration(config.Build());
    }

    public static void AddMoonCore(this ILoggingBuilder builder, Action<MoonCoreLoggingConfiguration>? configureAction = null)
    {
        builder.ClearProviders();

        var configuration = new MoonCoreLoggingConfiguration();
        
        if(configureAction != null)
            configureAction.Invoke(configuration);

        if (configuration.Console.Enable)
            builder.AddProvider(new ConsoleLoggingProvider(configuration.Console.EnableAnsiMode));

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

            builder.AddProvider(new FileLoggingProvider(configuration.FileLogging.Path));
        }
    }
}