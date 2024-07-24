using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class FileLogger : ILogger
{
    private readonly string CategoryName;
    private readonly StreamWriter StreamWriter;

    public FileLogger(string categoryName, StreamWriter streamWriter)
    {
        CategoryName = categoryName;
        StreamWriter = streamWriter;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);

        var shortName = LoggingConstants.ShortTextMappings[logLevel];
        
        StreamWriter.WriteLine($"{DateTime.Now:HH:mm:ss} {shortName} {CategoryName}: {message}");
    }
}