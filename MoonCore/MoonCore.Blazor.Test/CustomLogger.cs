using Spectre.Console;

namespace MoonCore.Blazor.Test;

public class CustomLogger : ILogger
{
    private readonly string _categoryName;

    public CustomLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) => null; // Not used in this example

    public bool IsEnabled(LogLevel logLevel) => true; // Enable all levels for simplicity

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);

        AnsiConsole.MarkupLine(FormatLogMessage(logLevel, _categoryName, message));
    }

    private string FormatLogMessage(LogLevel logLevel, string categoryName, string message)
    {
        string logLevelNameShort;
        string logLevelColor;

        switch (logLevel)
        {
            case LogLevel.Critical:
                logLevelNameShort = "CRIT";
                logLevelColor = "red1";
                break;
            
            case LogLevel.Error:
                logLevelNameShort = "ERRO";
                logLevelColor = "red";
                break;
            
            case LogLevel.Warning:
                logLevelNameShort = "WARN";
                logLevelColor = "yellow3_1";
                break;
            
            case LogLevel.Information:
                logLevelNameShort = "INFO";
                logLevelColor = "skyblue1";
                break;
            
            case LogLevel.Debug:
                logLevelNameShort = "DEBG";
                logLevelColor = "grey78";
                break;
            
            case LogLevel.Trace:
                logLevelNameShort = "TRCE";
                logLevelColor = "grey27";
                break;
            
            default:
                logLevelNameShort = "UNKN";
                logLevelColor = "grey27";
                break;
        }
        
        return $"[grey58]{DateTime.Now:HH:mm:ss}[/] [{logLevelColor} bold]{logLevelNameShort}[/] [grey78 italic]{categoryName}[/]: [white]{message}[/]";
    }
}