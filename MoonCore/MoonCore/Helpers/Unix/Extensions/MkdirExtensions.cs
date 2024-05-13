using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix.Extensions;

public static class MkdirExtensions
{
    public static UnixFsError? Mkdir(this UnixFileSystem fs, string path, FilePermissions permissions)
    {
        var error = fs.GetSafePath(path, out int parentDirectoryFd, out string fileName, out Action closeFd);

        if (error != null)
            return error;

        error = fs.MkdirAt(parentDirectoryFd, fileName, permissions);

        closeFd.Invoke();

        return error;
    }

    public static UnixFsError? MkdirAll(this UnixFileSystem fs, string path, FilePermissions permissions)
    {
        var normalized = fs.NormalizePath(path);

        return fs.Internal_MkdirAll(normalized, permissions);
    }

    private static UnixFsError? Internal_MkdirAll(this UnixFileSystem fs, string path, FilePermissions permissions)
    {
        var error = fs.LStat(path, out Stat stat);

        if (error == null)
        {
            if (fs.IsFileType(stat.st_mode, FilePermissions.S_IFLNK))
            {
                error = fs.Stat(path, out stat);

                if (error != null)
                    return error;
            }

            if (fs.IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
                return UnixFsError.NoError;

            return UnixFsError.BuildFromErrno($"Unknown file type: {stat.st_mode}", 0);
        }

        var lastIndex = path.Length;

        while (lastIndex > 0 && path[lastIndex - 1] == '/')
        {
            lastIndex--;
        }

        var parentIndex = lastIndex;

        while (parentIndex > 0 && path[parentIndex - 1] != '/')
        {
            parentIndex--;
        }

        if (parentIndex > 1)
        {
            var parentDir = path.Substring(0, parentIndex - 1);

            fs.Internal_MkdirAll(parentDir, permissions);
        }

        error = fs.Mkdir(path, permissions);

        if (error != null)
        {
            var lStatError = fs.LStat(path, out Stat lstat);

            if (lStatError == null && fs.IsFileType(lstat.st_mode, FilePermissions.S_IFDIR))
                return UnixFsError.NoError;

            return error;
        }

        return UnixFsError.NoError;
    }
    
    private static UnixFsError? MkdirAt(this UnixFileSystem fs, int parentDirectoryFd, string path, FilePermissions permissions)
    {
        Syscall.mkdirat(parentDirectoryFd, path, permissions);

        return UnixFsError.AutoHandle("An error occured in mkdirat");
    }
}