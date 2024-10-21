namespace MoonCore.Extended.Exceptions;

public class JwtClaimException : Exception
{
    public JwtClaimException()
    {
    }

    public JwtClaimException(string message) : base(message)
    {
    }

    public JwtClaimException(string message, Exception inner) : base(message, inner)
    {
    }
}