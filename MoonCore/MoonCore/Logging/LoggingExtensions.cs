using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public static class LoggingExtensions
{
    /// <summary>
    /// Adds a console logging provider which uses ansi color escape codes to color code the output 
    /// </summary>
    /// <param name="loggerFactory">Logging factory to add the provider to</param>
    public static void AddAnsiConsole(this ILoggerFactory loggerFactory)
     => loggerFactory.AddProvider(new AnsiConsoleLoggerProvider());

    /// <summary>
    /// Adds a console logging provider which uses ansi color escape codes to color code the output 
    /// </summary>
    /// <param name="builder">Logging builder to add the provider to</param>
    public static void AddAnsiConsole(this ILoggingBuilder builder)
        => builder.AddProvider(new AnsiConsoleLoggerProvider());

    /// <summary>
    /// Adds a file logging provider which appends the logging messages into the provided path
    /// </summary>
    /// <param name="loggerFactory">Logging factory to add the provider to</param>
    /// <param name="path">Path to write the log messages to</param>
    public static void AddFile(this ILoggerFactory loggerFactory, string path)
        => loggerFactory.AddProvider(new FileLoggingProvider(path));

    /// <summary>
    /// Adds a file logging provider which appends the logging messages into the provided path
    /// </summary>
    /// <param name="builder">Logging builder to add the provider to</param>
    /// <param name="path">Path to write the log messages to</param>
    public static void AddFile(this ILoggingBuilder builder, string path)
        => builder.AddProvider(new FileLoggingProvider(path));
}