namespace MoonCore.Blazor.FlyonUi.Exceptions;

/// <summary>
/// This exception is used to communicate error messages to components and error handlers
/// </summary>
public class DisplayException : Exception
{
    public DisplayException()
    {
    }

    public DisplayException(string message) : base(message)
    {
    }

    public DisplayException(string message, Exception inner) : base(message, inner)
    {
    }
}