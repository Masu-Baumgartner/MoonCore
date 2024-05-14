using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix.Extensions;

public static class RenameExtensions
{
    public static UnixFsError? Rename(this UnixFileSystem fs, string oldPath, string newPath)
    {
        if (oldPath == newPath)
            return UnixFsError.NoError;

        var error = fs.GetSafePath(oldPath, out int oldParentDirectoryFd, out string oldParentFileName,
            out Action oldCloseFd);

        if (error != null)
        {
            oldCloseFd.Invoke();
            return error;
        }

        if (oldParentFileName == ".")
            return UnixFsError.BuildFromErrno("Unable to rename root", 0);

        error = fs.LStatAt(oldParentDirectoryFd, oldParentFileName, out _);

        if (error != null)

        {
            oldCloseFd.Invoke();
            return error;
        }

        error = fs.GetSafePath(newPath, out int newParentDirectoryFd, out string newParentFileName,
            out Action newCloseFd);

        if (error != null)
        {
            newCloseFd.Invoke();

            if (error.Errno != Errno.ENOENT)
                return error;

            var parentFolderDir = Formatter.ReplaceEnd(newPath, newParentFileName, "").TrimEnd('/');
            error = fs.MkdirAll(parentFolderDir, FilePermissions.ACCESSPERMS);

            if (error != null)
            {
                newCloseFd.Invoke();
                oldCloseFd.Invoke();
                return error;
            }

            error = fs.GetSafePath(newPath, out newParentDirectoryFd, out newParentFileName,
                out newCloseFd);

            if (error != null)
            {
                newCloseFd.Invoke();
                oldCloseFd.Invoke();
                return error;
            }
        }

        if (newParentFileName == ".")
            return UnixFsError.BuildFromErrno("Unable to rename root", 0);

        error = fs.LStatAt(newParentDirectoryFd, newParentFileName, out _);

        if (error == null)
            return UnixFsError.BuildFromErrno("Target exists", Errno.EEXIST);
        else if (error.Errno != Errno.ENOENT)
            return error;

        Syscall.renameat(oldParentDirectoryFd, oldParentFileName, newParentDirectoryFd, newParentFileName);

        newCloseFd.Invoke();
        oldCloseFd.Invoke();

        return UnixFsError.AutoHandle("Error while renaming");
    }
}