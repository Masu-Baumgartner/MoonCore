using Mono.Unix.Native;

namespace MoonCore.Unix.Extensions;

public static class RemoveExtensions
{
    public static UnixFsError? RemoveAll(this UnixFileSystem fs, string path)
    {
        var normalized = fs.NormalizePath(path);

        if (normalized == ".")
            return UnixFsError.BuildFromErrno("Path error", Errno.EINVAL);

        return fs.Internal_RemoveAll(path);
    }

    private static UnixFsError? Internal_RemoveAll(this UnixFileSystem fs, string path)
    {
        // Prevent invalid values
        if (string.IsNullOrEmpty(path))
            return UnixFsError.NoError;

        if (path.EndsWith("."))
            return UnixFsError.BuildFromErrno("Ends with dot", Errno.EINVAL);

        // Try to remove it
        var fileRmError = fs.Remove(path);

        if (fileRmError == null || fileRmError.Errno == Errno.ENOENT)
            return UnixFsError.NoError;

        // If the rm failed, we need to check how we can rm it so we lstat
        var error = fs.LStat(path, out var stat);

        if (error != null)
            return error;

        // not a directory? idk how to rm, so we return the initial rm error
        if (!fs.IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
            return fileRmError;

        // read the contents of the directory
        error = fs.ReadDir(path, out var items);

        if (error != null)
            return error;

        // the directory is empty but the above rm failed? unknown error => return the initial one
        if (items.Length == 0)
            return fileRmError;

        // Delete recursively and stop if any error has been encountered
        foreach (var entry in items)
        {
            var normalizedSubPath = fs.NormalizePath(path + "/" + entry.Name);
            var recursiveRmError = fs.Internal_RemoveAll(normalizedSubPath);

            if (recursiveRmError != null)
                return recursiveRmError;
        }

        // Now that the contents of the directory are deleted, we can try to delete this item we initially wanted to delete
        fileRmError = fs.Remove(path);

        if (fileRmError == null || fileRmError.Errno == Errno.ENOENT)
            return UnixFsError.NoError;

        return fileRmError;
    }

    public static UnixFsError? Remove(this UnixFileSystem fs, string path)
    {
        var error = fs.GetSafePath(path, out var parentDirectoryFd, out var fileName, out var closeFd);

        if (error != null)
            return error;

        if (fileName == ".")
            return UnixFsError.BuildFromErrno("Path error", Errno.EINVAL);

        error = fs.UnlinkAt(parentDirectoryFd, fileName, 0);

        if (error == null)
            return UnixFsError.NoError;

        var dirError = fs.UnlinkAt(parentDirectoryFd, fileName, AtFlags.AT_REMOVEDIR);

        if (dirError == null)
            return UnixFsError.NoError;

        if (dirError.Errno != Errno.ENOTDIR)
            error = dirError;

        return error;
    }

    public static UnixFsError? UnlinkAt(this UnixFileSystem fs, int parentDirectoryFd, string fileName, AtFlags flags)
    {
        Syscall.unlinkat(parentDirectoryFd, fileName, flags);

        return UnixFsError.AutoHandle("unlink at");
    }
}