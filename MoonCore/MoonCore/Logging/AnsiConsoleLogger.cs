using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class AnsiConsoleLogger : ILogger
{
    private readonly string CategoryName;

    public AnsiConsoleLogger(string categoryName)
    {
        CategoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var message = formatter(state, exception);

        var shortName = LoggingConstants.ShortTextMappings[logLevel];
        var color = LoggingConstants.ColorMappings[logLevel];

        Console.Write(Crayon.Output.Rgb(148, 148, 148).Text($"{DateTime.Now:dd.MM.yy HH:mm:ss} "));
        Console.Write(Crayon.Output.Rgb(color.Item1, color.Item2, color.Item3).Bold(shortName + " "));
        Console.Write(Crayon.Output.Rgb(198, 198, 198).Text(CategoryName));
        Console.WriteLine(Crayon.Output.Rgb(255, 255, 255).Text($": {message}"));

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (exception != null)
        {
            Console.WriteLine(
                Crayon.Output.Rgb(255, 255, 255).Text(exception.ToString())
            );
        }
    }
}