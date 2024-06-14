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
            return new ConsoleLogger(categoryName);
        
        return new AnsiLogger(categoryName);
    }

    public void Dispose()
    {
        // Implement disposal logic if necessary
    }
}