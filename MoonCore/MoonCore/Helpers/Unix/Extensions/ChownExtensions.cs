using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix.Extensions;

public static class ChownExtensions
{
    public static UnixFsError? Chown(this UnixFileSystem fs, string path, int uid, int gid)
    {
        return fs.Internal_FChown(path, uid, gid, 0);
    }

    public static UnixFsError? Internal_FChown(this UnixFileSystem fs, string path, int uid, int gid, AtFlags flags)
    {
        var error = fs.GetSafePath(path, out var parentDirectoryFd, out var fileName, out var closeFd);

        if (error != null)
            return error;

        Syscall.fchownat(parentDirectoryFd, fileName, uid, gid, flags);
        error = UnixFsError.AutoHandle();
        
        closeFd.Invoke();

        return error;
    }
}