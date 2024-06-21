namespace MoonCore.Blazor.Test;

public class CustomLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new CustomLogger(categoryName);
    }

    public void Dispose()
    {
        // Implement disposal logic if necessary
    }
}