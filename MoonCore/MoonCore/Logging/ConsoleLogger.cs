using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class ConsoleLogger : ILogger
{
    private readonly string CategoryName;

    public ConsoleLogger(string categoryName)
    {
        CategoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);

        var shortText = LoggingConstants.ShortTextMappings[logLevel];
        
        Console.WriteLine($"{DateTime.Now:dd.MM.yy HH:mm:ss} {shortText} {CategoryName}: {message}");
    }
}