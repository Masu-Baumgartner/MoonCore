using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class ConsoleLoggingProvider : ILoggerProvider
{
    private readonly bool EnableAnsi;

    public ConsoleLoggingProvider(bool enableAnsi)
    {
        EnableAnsi = enableAnsi;
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (EnableAnsi)
            return new AnsiConsoleLogger(categoryName);
        
        return new ConsoleLogger(categoryName);
    }

    public void Dispose()
    {
        // Implement disposal logic if necessary
    }
}