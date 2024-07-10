using Mono.Unix.Native;

namespace MoonCore.Unix.Extensions;

public static class ChmodExtensions
{
    public static UnixFsError? Chmod(this UnixFileSystem fs, string path, FilePermissions permissions)
    {
        var error = fs.GetSafePath(path, out var parentDirectoryFd, out var fileName, out var closeFd);

        if (error != null)
            return error;

        Syscall.fchmodat(parentDirectoryFd, fileName, permissions, 0);
        error = UnixFsError.AutoHandle();
        
        closeFd.Invoke();

        return error;
    }
}