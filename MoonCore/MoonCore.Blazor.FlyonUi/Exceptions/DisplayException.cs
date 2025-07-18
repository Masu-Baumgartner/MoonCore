namespace MoonCore.Blazor.FlyonUi.Exceptions;

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