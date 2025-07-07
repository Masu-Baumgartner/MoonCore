using Microsoft.Extensions.Logging;

namespace MoonCore.Logging;

public class AnsiConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
        => new AnsiConsoleLogger(categoryName);

    public void Dispose()
    {
    }
}