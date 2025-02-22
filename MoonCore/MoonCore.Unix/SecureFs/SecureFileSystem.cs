using Mono.Unix;
using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem : IDisposable
{
    public bool IsDisposed => RootFileDescriptor == -1;

    private readonly string RootPath;
    private int RootFileDescriptor;

    public SecureFileSystem(string rootPath)
    {
        RootPath = rootPath;

        RootFileDescriptor = Syscall.open(RootPath, OpenFlags.O_RDONLY | OpenFlags.O_DIRECTORY, 0);
    }

    private int OpenAt(int baseDirFd, string path, OpenFlags openFlags, FilePermissions filePermissions)
    {
        // Ensure O_CLOEXEC flag
        if (!openFlags.HasFlag(OpenFlags.O_CLOEXEC))
            openFlags |= OpenFlags.O_CLOEXEC;

        var fileDescriptor = Syscall.openat(baseDirFd, path, openFlags, filePermissions);

        // Handle possible errors
        var lastError = Stdlib.GetLastError();

        if (lastError != 0)
        {
            switch (lastError)
            {
                case Errno.ENOENT:
                    throw new SyscallException(lastError, "No file or directory found using the provided path");

                case Errno.EBADF:
                    throw new SyscallException(lastError, "Bad parent file descriptor");

                case Errno.ELOOP:
                    throw new SyscallException(lastError,
                        "Symlink detected in path while symlink following has been disabled");

                default:
                    throw new SyscallException(lastError);
            }
        }

        // Validate opened file descriptor
        var fdTarget = UnixPath.ReadLink($"/proc/self/fd/{fileDescriptor}");

        if (!fdTarget.StartsWith(RootPath))
        {
            Syscall.close(fileDescriptor);
            throw new SyscallException(Errno.ELOOP, "Target of file descriptor is outside of the root directory");
        }

        return fileDescriptor;
    }

    private void OpenEntrySafe(string path, Action<int, string> handle)
    {
        var parentDir = Path.GetDirectoryName(path) ?? null;
        var fileName = Path.GetFileName(path);

        int parentFd;

        if (string.IsNullOrEmpty(parentDir))
            parentFd = RootFileDescriptor;
        else
            parentFd = OpenAt(RootFileDescriptor, parentDir, OpenFlags.O_RDONLY | OpenFlags.O_DIRECTORY, 0);

        // Let handle function use the parent fd to call the *at functions (e.g. mkdirat)
        try
        {
            handle.Invoke(parentFd, fileName);
            
            // We only want to close newly opened fds
            if(parentFd != RootFileDescriptor)
                Syscall.close(parentFd);
        }
        catch (Exception)
        {
            // We only want to close newly opened fds
            if(parentFd != RootFileDescriptor)
                Syscall.close(parentFd);
            
            throw;
        }
    }

    private bool IsFileType(FilePermissions mode, FilePermissions type)
        => (mode & FilePermissions.S_IFMT) == type;

    public void Dispose()
    {
        Syscall.close(RootFileDescriptor);
        RootFileDescriptor = -1;
    }
}