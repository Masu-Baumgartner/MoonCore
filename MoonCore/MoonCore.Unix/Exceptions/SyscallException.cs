using Mono.Unix.Native;

namespace MoonCore.Unix.Exceptions;

public class SyscallException : Exception
{
    public Errno Errno { get; private set; }

    public SyscallException(Errno errno) : base($"A syscall returned an error: {errno.ToString()}")
    {
        Errno = errno;
    }

    public SyscallException(Errno errno, string message) : base(message)
    {
        Errno = errno;
    }
}