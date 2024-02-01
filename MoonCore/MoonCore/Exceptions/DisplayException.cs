namespace MoonCore.Exceptions;

/// <summary>
/// This exception is used to trigger the ui showing an error message to the user and stop the current operation. The message shown to the user is the exception message
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