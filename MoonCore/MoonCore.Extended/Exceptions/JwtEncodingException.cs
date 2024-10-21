namespace MoonCore.Extended.Exceptions;

public class JwtEncodingException : Exception
{
    public JwtEncodingException()
    {
    }

    public JwtEncodingException(string message) : base(message)
    {
    }

    public JwtEncodingException(string message, Exception inner) : base(message, inner)
    {
    }
}