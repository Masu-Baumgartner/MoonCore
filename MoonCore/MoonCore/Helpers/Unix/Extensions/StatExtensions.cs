using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix.Extensions;

public static class StatExtensions
{
    public static UnixFsError? Stat(this UnixFileSystem fs, string path, out Stat stat)
    {
        return fs.Internal_FStat(path, 0, out stat);
    }

    public static UnixFsError? LStat(this UnixFileSystem fs, string path, out Stat stat)
    {
        return fs.Internal_FStat(path, AtFlags.AT_SYMLINK_NOFOLLOW, out stat);
    }

    public static UnixFsError? LStatAt(this UnixFileSystem fs, int parentDirectoryFd, string path, out Stat stat)
    {
        return fs.Internal_FStatAt(parentDirectoryFd, path, AtFlags.AT_SYMLINK_NOFOLLOW, out stat);
    }

    private static UnixFsError? Internal_FStat(this UnixFileSystem fs, string path, AtFlags flags, out Stat stat)
    {
        var error = fs.GetSafePath(path, out int parentDirectoryFd, out string fileName, out Action closeFd);

        if (error != null)
        {
            stat = default;
            return error;
        }

        closeFd.Invoke();

        return fs.Internal_FStatAt(parentDirectoryFd, fileName, flags, out stat);
    }

    private static UnixFsError? Internal_FStatAt(this UnixFileSystem fs, int parentDirectoryFd, string name, AtFlags flags, out Stat stat)
    {
        Syscall.fstatat(parentDirectoryFd, name, out stat, flags);

        return UnixFsError.AutoHandle("An error occured in fstatat");
    }
}