using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix;

public class UnixFsException : Exception
{
    public Errno Errno { get; set; }

    public UnixFsException()
    {
    }

    public UnixFsException(string message) : base(message)
    {
    }

    public UnixFsException(string message, Exception inner) : base(message, inner)
    {
    }
}