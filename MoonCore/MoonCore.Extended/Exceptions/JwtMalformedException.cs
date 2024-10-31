namespace MoonCore.Extended.Exceptions;

public class JwtMalformedException : Exception
{
    public JwtMalformedException()
    {
    }

    public JwtMalformedException(string message) : base(message)
    {
    }

    public JwtMalformedException(string message, Exception inner) : base(message, inner)
    {
    }
}