using Microsoft.Win32.SafeHandles;
using Mono.Unix;
using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix.Extensions;

public static class OpenExtensions
{
    public static UnixFsError? Open(this UnixFileSystem fs, string path, out SafeFileHandle handle)
    {
        return fs.OpenFile(path, OpenFlags.O_RDONLY, 0, out handle);
    }

    public static UnixFsError? OpenFile(this UnixFileSystem fs, string path, OpenFlags flags,
        FilePermissions permissions, out SafeFileHandle handle)
    {
        var error = fs.Internal_OpenFile(path, flags, permissions, out int fd);

        if (error != null)
        {
            handle = default!;
            return error;
        }

        handle = new SafeFileHandle(new IntPtr(fd), true);

        return UnixFsError.NoError;
    }

    private static UnixFsError? Internal_OpenFile(this UnixFileSystem fs, string path, OpenFlags flags,
        FilePermissions permissions, out int fd)
    {
        var error = fs.GetSafePath(path, out var parentDirectoryFd, out var fileName, out var closeFd);
        
        if (error != null)
        {
            fd = 0;
            return error;
        }

        error = fs.OpenAt(parentDirectoryFd, fileName, flags, permissions, out fd);
        
        closeFd.Invoke();

        return error;
    }

    public static UnixFsError? OpenAt(this UnixFileSystem fs, int parentDirectoryFd, string path, OpenFlags flags,
        FilePermissions permissions,
        out int fileDescriptor)
    {
        if (!flags.HasFlag(OpenFlags.O_NOFOLLOW))
            flags |= OpenFlags.O_NOFOLLOW;

        var openAtError = fs.Internal_OpenAt(parentDirectoryFd, path, flags, permissions, out fileDescriptor);

        if (openAtError != null)
            return openAtError;

        // Validate file descriptor
        var finalPath = UnixPath.ReadLink("/proc/self/fd/" + fileDescriptor);

        var error = Stdlib.GetLastError();

        if (error != 0)
            return UnixFsError.BuildFromErrno($"Error reading symlink for fd {fileDescriptor}", error);

        if (!finalPath.StartsWith(fs.BaseDirectory))
            throw new UnauthorizedAccessException(finalPath);

        return UnixFsError.NoError;
    }

    private static UnixFsError? Internal_OpenAt(this UnixFileSystem fs, int parentDirectoryFd, string path,
        OpenFlags flags,
        FilePermissions permissions,
        out int fileDescriptor)
    {
        if (!flags.HasFlag(OpenFlags.O_CLOEXEC))
            flags |= OpenFlags.O_CLOEXEC;

        fileDescriptor = Syscall.openat(parentDirectoryFd, path, flags, permissions);

        var error = Stdlib.GetLastError();

        if (error != 0)
        {
            if (error == Errno.ELOOP)
                throw new UnauthorizedAccessException(path);

            return UnixFsError.BuildFromErrno(error);
        }

        return UnixFsError.NoError;
    }
    
    public static UnixFsError? OpenFileAt(this UnixFileSystem fs, int parentDirectoryFd, string path, OpenFlags flags, FilePermissions permissions,
        out SafeFileHandle handle)
    {
        var error = fs.OpenAt(parentDirectoryFd, path, flags, permissions, out int fileDescriptor);

        if (error != null)
        {
            handle = default!;
            return error;
        }

        handle = new SafeFileHandle(new IntPtr(fileDescriptor), true);
        return UnixFsError.NoError;
    }
}