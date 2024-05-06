namespace MoonCore.Exceptions;

public class UnsafeAccessException : Exception
{
    public UnsafeAccessException()
    {
    }

    public UnsafeAccessException(string message) : base(message)
    {
    }

    public UnsafeAccessException(string message, Exception inner) : base(message, inner)
    {
    }
}