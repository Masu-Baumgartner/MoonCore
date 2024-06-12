namespace MoonCore.Blazor.Exceptions;

public class PayloadTooLargeException : Exception
{
    public PayloadTooLargeException()
    {
    }

    public PayloadTooLargeException(string message) : base(message)
    {
    }

    public PayloadTooLargeException(string message, Exception inner) : base(message, inner)
    {
    }
}