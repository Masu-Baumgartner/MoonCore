using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using MoonCore.Helpers;

namespace MoonCore.Unix.Extensions;

public static class TouchExtensions
{
    public static UnixFsError? Touch(this UnixFileSystem fs, string path, OpenFlags flags, FilePermissions permissions, out FileStream fileStream)
    {
        if (!flags.HasFlag(OpenFlags.O_CREAT))
            flags |= OpenFlags.O_CREAT;

        int parentDirectoryFd;
        string fileName;
        Action closeFd;
        fileStream = default!;

        var error = fs.GetSafePath(path, out parentDirectoryFd, out fileName, out closeFd);

        if (error != null)
        {
            closeFd.Invoke();

            var parentFolderDir = Formatter.ReplaceEnd(path, fileName, "").TrimEnd('/');
            error = fs.MkdirAll(parentFolderDir, FilePermissions.ACCESSPERMS);

            if (error != null)
                return error;

            error = fs.GetSafePath(path, out parentDirectoryFd, out fileName, out closeFd);

            if (error != null)
                return error;
        }

        error = fs.OpenFileAt(parentDirectoryFd, fileName, flags, permissions, out SafeFileHandle handle);

        if (error != null)
            return error;

        fileStream = new FileStream(handle, FileAccess.ReadWrite);

        return UnixFsError.NoError;
    }
}