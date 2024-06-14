using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class ConsoleLoggingProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new AnsiLogger(categoryName);
    }

    public void Dispose()
    {
        // Implement disposal logic if necessary
    }
}