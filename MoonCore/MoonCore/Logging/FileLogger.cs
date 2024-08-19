using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class FileLogger : ILogger
{
    private readonly string CategoryName;
    private readonly TextWriter TextWriter;

    private int WriteCounter = 0;
    private int WriteFlushLimit;

    public FileLogger(string categoryName, TextWriter textWriter, int writeFlushLimit)
    {
        CategoryName = categoryName;
        TextWriter = textWriter;
        WriteFlushLimit = writeFlushLimit;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);

        var shortName = LoggingConstants.ShortTextMappings[logLevel];
        
        TextWriter.WriteLine($"{DateTime.Now:HH:mm:ss} {shortName} {CategoryName}: {message}");
        WriteCounter++;

        if (WriteCounter <= WriteFlushLimit)
            return;
        
        WriteCounter = 0;
        TextWriter.Flush();
    }
}