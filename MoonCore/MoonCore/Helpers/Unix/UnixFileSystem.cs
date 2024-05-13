// Note:
// This class helps with interacting with a unix file system in a symlink safe way in order to ensure isolation.
// An article describing the issue this solves can be found here: https://lwn.net/Articles/899543/
// This code is more or less a port from the implementation for wings, the pterodactyl daemon
// Their (MIT licensed) implementation can be found here: https://github.com/pterodactyl/wings/blob/develop/internal/ufs/fs_unix.go
// Credits go the the original author(s):
// - Copyright (c) 2024 Matthew Penner


using Microsoft.Win32.SafeHandles;
using Mono.Unix;
using Mono.Unix.Native;
using MoonCore.Helpers.Unix.Extensions;

namespace MoonCore.Helpers.Unix;

public class UnixFileSystem : IDisposable
{
    public readonly string BaseDirectory;
    public readonly int BaseDirectoryFd;

    public UnixFileSystem(string baseDirectory)
    {
        BaseDirectory = baseDirectory;

        BaseDirectoryFd = Syscall.open(BaseDirectory, OpenFlags.O_DIRECTORY | OpenFlags.O_RDONLY);
    }

    private UnixFsError? OpenFdAt(int directoryFd, string path, out SafeFileHandle handle)
    {
        int fd;

        while (true)
        {
            var error = this.OpenAt(directoryFd, path, OpenFlags.O_RDONLY | OpenFlags.O_CLOEXEC | OpenFlags.O_NOFOLLOW, 0, out fd);
            
            if(error == null)
                break;
            
            if(error.Errno == Errno.EINTR)
                continue;

            handle = default!;
            return error;
        }

        handle = new SafeFileHandle(new IntPtr(fd), true);
        return UnixFsError.NoError;
    }
    
    private void SplitPath(string path, out string parentDirectory, out string baseDirectory)
    {
        // If no better parent is found, the path is relative from "here"
        string dirname = ".";

        // Remove all but one leading slash.
        while (path.Length > 1 && path[0] == '/' && path[1] == '/')
        {
            path = path.Substring(1);
        }

        int i = path.Length - 1;

        // Remove trailing slashes.
        for (; i > 0 && path[i] == '/'; i--)
        {
            path = path.Substring(0, i);
        }

        // If no slashes in path, base is path
        string basename = path;

        // Remove leading directory path
        for (i--; i >= 0; i--)
        {
            if (path[i] == '/')
            {
                if (i == 0)
                {
                    dirname = path.Substring(0, 1);
                }
                else
                {
                    dirname = path.Substring(0, i);
                }
                basename = path.Substring(i + 1);
                break;
            }
        }

        parentDirectory = dirname;
        baseDirectory = basename;
    }

    public UnixFsError? GetSafePath(string path, out int parentDirectoryFd, out string fileName, out Action closeFd)
    {
        closeFd = () => { };

        var normalized = NormalizePath(path);

        // Check if base directory fd is loaded

        fileName = Path.GetFileName(normalized);
        var parentDirectoryPath = Formatter.ReplaceEnd(normalized, fileName, "");

        if (string.IsNullOrEmpty(parentDirectoryPath))
        {
            parentDirectoryFd = BaseDirectoryFd;
            return UnixFsError.NoError;
        }

        var openAtResult = this.OpenAt(BaseDirectoryFd, parentDirectoryPath, OpenFlags.O_DIRECTORY | OpenFlags.O_RDONLY, 0,
            out int fd);
        parentDirectoryFd = fd;

        if (parentDirectoryFd != 0)
            closeFd = () => Syscall.close(fd);

        return openAtResult;
    }

    public string NormalizePath(string input)
    {
        return input
            .Replace("//", "/")
            .Replace("..", "")
            .TrimStart('/');
    }

    public bool IsFileType(FilePermissions mode, FilePermissions type)
    {
        return (mode & FilePermissions.S_IFMT) == type;
    }

    public void Dispose()
    {
        Syscall.close(BaseDirectoryFd);
    }
}