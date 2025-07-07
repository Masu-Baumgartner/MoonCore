using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public static class LoggingExtensions
{
    public static void AddAnsiConsole(this ILoggerFactory loggerFactory)
     => loggerFactory.AddProvider(new AnsiConsoleLoggerProvider());

    public static void AddAnsiConsole(this ILoggingBuilder builder)
        => builder.AddProvider(new AnsiConsoleLoggerProvider());

    public static void AddFile(this ILoggerFactory loggerFactory, string path)
        => loggerFactory.AddProvider(new FileLoggingProvider(path));

    public static void AddFile(this ILoggingBuilder builder, string path)
        => builder.AddProvider(new FileLoggingProvider(path));
}