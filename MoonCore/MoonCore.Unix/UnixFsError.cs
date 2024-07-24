using Mono.Unix.Native;

namespace MoonCore.Unix;

public class UnixFsError
{
    public Errno Errno { get; set; }
    public string Message { get; set; } = "";

    public static UnixFsError? NoError = null;

    public static UnixFsError GetLastError() => GetLastError("");

    public static UnixFsError? AutoHandle(string? message = null)
    {
        var error = Syscall.GetLastError();

        if (error == 0)
            return NoError;

        return BuildFromErrno(message ?? "", error);
    }

    public static UnixFsError GetLastError(string customMessage)
    {
        var error = new UnixFsError()
        {
            Errno = Syscall.GetLastError(),
            Message = customMessage
        };

        return error;
    }

    public static UnixFsError BuildFromErrno(string customMessage, Errno errno)
    {
        var error = new UnixFsError()
        {
            Errno = errno,
            Message = customMessage
        };

        return error;
    }

    public static UnixFsError BuildFromErrno(Errno errno)
    {
        return BuildFromErrno("", errno);
    }
}