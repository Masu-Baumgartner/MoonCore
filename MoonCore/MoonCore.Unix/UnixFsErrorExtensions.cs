namespace MoonCore.Unix;

public static class UnixFsErrorExtensions
{
    public static void ThrowIfError(this UnixFsError? error)
    {
        if(error == null)
            return;

        throw new UnixFsException(string.IsNullOrEmpty(error.Message) ? $"An unhanded unix fs error occured: {error.Errno}" : error.Message + $" ({error.Errno})")
        {
            Errno = error.Errno
        };
    }
}