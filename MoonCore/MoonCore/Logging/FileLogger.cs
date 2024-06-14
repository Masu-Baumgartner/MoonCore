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

        StreamWriter.WriteLine(FormatLogMessage(logLevel, CategoryName, message));
    }

    private static string FormatLogMessage(LogLevel logLevel, string categoryName, string message)
    {
        string logLevelNameShort;

        switch (logLevel)
        {
            case LogLevel.Critical:
                logLevelNameShort = "CRIT";
                break;
            
            case LogLevel.Error:
                logLevelNameShort = "ERRO";
                break;
            
            case LogLevel.Warning:
                logLevelNameShort = "WARN";
                break;
            
            case LogLevel.Information:
                logLevelNameShort = "INFO";
                break;
            
            case LogLevel.Debug:
                logLevelNameShort = "DEBG";
                break;
            
            case LogLevel.Trace:
                logLevelNameShort = "TRCE";
                break;
            
            default:
                logLevelNameShort = "UNKN";
                break;
        }
        
        return $"{DateTime.Now:HH:mm:ss} {logLevelNameShort} {categoryName}: {message}";
    }
}