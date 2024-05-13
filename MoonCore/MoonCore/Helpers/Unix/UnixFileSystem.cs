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

namespace MoonCore.Helpers.Unix;

public class UnixFileSystem
{
    private readonly string BaseDirectory;
    private readonly int BaseDirectoryFd;

    public UnixFileSystem(string baseDirectory)
    {
        BaseDirectory = baseDirectory;

        BaseDirectoryFd = Syscall.open(BaseDirectory, OpenFlags.O_DIRECTORY | OpenFlags.O_RDONLY);
    }

    public UnixFsError? RemoveAll(string path)
    {
        var normalized = NormalizePath(path);

        if (normalized == ".")
            return UnixFsError.BuildFromErrno("Path error", Errno.EINVAL);

        return Internal_RemoveAll(path);
    }

    public UnixFsError? Internal_RemoveAll(string path)
    {
        // Prevent invalid values
        if(string.IsNullOrEmpty(path))
            return UnixFsError.NoError;
        
        if(path.EndsWith("."))
            return UnixFsError.BuildFromErrno("Ends with dot", Errno.EINVAL);

        // Try to remove it
        var fileRmError = Remove(path);
        
        if(fileRmError == null || fileRmError.Errno == Errno.ENOENT)
            return UnixFsError.NoError;

        // If the rm failed, we need to check how we can rm it so we lstat
        var error = LStat(path, out var stat);

        if (error != null)
            return error;

        // not a directory? idk how to rm, so we return the initial rm error
        if (!IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
            return fileRmError;

        // read the contents of the directory
        error = ReadDir(path, out var items);

        if (error != null)
            return error;

        // the directory is empty but the above rm failed? unknown error => return the initial one
        if (items.Length == 0)
            return fileRmError;

        // Delete recursively and stop if any error has been encountered
        foreach (var entry in items)
        {
            var normalizedSubPath = NormalizePath(path + "/" + entry.Name);
            var recursiveRmError = Internal_RemoveAll(normalizedSubPath);

            if (recursiveRmError != null)
                return recursiveRmError;
        }
        
        // Now that the contents of the directory are deleted, we can try to delete this item we initially wanted to delete
        fileRmError = Remove(path);
        
        if(fileRmError == null || fileRmError.Errno == Errno.ENOENT)
            return UnixFsError.NoError;
        
        return fileRmError;
    }

    public UnixFsError? OpenFdAt(int directoryFd, string path, out SafeFileHandle handle)
    {
        int fd;

        while (true)
        {
            var error = OpenAt(directoryFd, path, OpenFlags.O_RDONLY | OpenFlags.O_CLOEXEC | OpenFlags.O_NOFOLLOW, 0, out fd);
            
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

    public UnixFsError? Open(string path, out SafeFileHandle handle)
    {
        return OpenFile(path, OpenFlags.O_RDONLY, 0, out handle);
    }

    public UnixFsError? OpenFile(string path, OpenFlags flags, FilePermissions permissions, out SafeFileHandle handle)
    {
        var error = Internal_OpenFile(path, flags, permissions, out int fd);

        if (error != null)
        {
            handle = default!;
            return error;
        }

        handle = new SafeFileHandle(new IntPtr(fd), true);
        
        return UnixFsError.NoError;
    }

    public UnixFsError? Internal_OpenFile(string path, OpenFlags flags, FilePermissions permissions, out int fd)
    {
        var error = GetSafePath(path, out var parentDirectoryFd, out var fileName, out var closeFd);

        closeFd.Invoke();

        if (error != null)
        {
            fd = 0;
            return error;
        }

        return OpenAt(parentDirectoryFd, fileName, flags, permissions, out fd);
    }

    public UnixFsError? Remove(string path)
    {
        var error = GetSafePath(path, out var parentDirectoryFd, out var fileName, out var closeFd);

        if (error != null)
            return error;
        
        if(fileName == ".")
            return UnixFsError.BuildFromErrno("Path error", Errno.EINVAL);

        error = UnlinkAt(parentDirectoryFd, fileName, 0);
        
        if(error == null)
            return UnixFsError.NoError;

        var dirError = UnlinkAt(parentDirectoryFd, fileName, AtFlags.AT_REMOVEDIR);
        
        if(dirError == null)
            return UnixFsError.NoError;

        if (dirError.Errno != Errno.ENOTDIR)
            error = dirError;

        return error;
    }

    public UnixFsError? UnlinkAt(int parentDirectoryFd, string fileName, AtFlags flags)
    {
        Syscall.unlinkat(parentDirectoryFd, fileName, flags);
        
        return UnixFsError.AutoHandle("unlink at");
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

    public UnixFsError? ReadDir(string path, out UnixFsEntry[] stats)
    {
        stats = Array.Empty<UnixFsEntry>();

        var error = GetSafePath(path, out int parentDirectoryFd, out string fileName, out Action closeFd);

        if (error != null)
            return error;

        error = OpenAt(parentDirectoryFd, fileName, OpenFlags.O_DIRECTORY | OpenFlags.O_RDONLY, 0,
            out int fileDescriptor);

        if (error != null && fileName != "")
            return error;

        error = Internal_Readdir(fileName == "" ? parentDirectoryFd : fileDescriptor, out stats);

        closeFd.Invoke();
        Syscall.close(fileDescriptor);

        return error;
    }

    public UnixFsError? Internal_Readdir(int fd, out UnixFsEntry[] stats)
    {
        var directoryFd = Syscall.fdopendir(fd);

        var lastError = Syscall.GetLastError();

        if (lastError != 0)
        {
            stats = Array.Empty<UnixFsEntry>();
            return UnixFsError.BuildFromErrno("fd open dir", lastError);
        }

        Dirent? currentDir;
        List<UnixFsEntry> entries = new();

        do
        {
            currentDir = Syscall.readdir(directoryFd);

            var readDirError = Syscall.GetLastError();

            if (readDirError != 0)
            {
                stats = Array.Empty<UnixFsEntry>();
                return UnixFsError.BuildFromErrno("readdir", readDirError);
            }

            if (currentDir != null)
            {
                if (currentDir.d_name == "." || currentDir.d_name == "..")
                    continue;

                var error = LStatAt(fd, currentDir.d_name, out var stat);

                if (error != null)
                {
                    stats = Array.Empty<UnixFsEntry>();
                    return error;
                }

                entries.Add(new()
                {
                    Name = currentDir.d_name,
                    IsDirectory = IsFileType(stat.st_mode, FilePermissions.S_IFDIR),
                    IsFile = IsFileType(stat.st_mode, FilePermissions.S_IFREG),
                    Size = stat.st_size,
                    LastChanged = DateTimeOffset.FromUnixTimeSeconds(stat.st_mtime).UtcDateTime,
                    CreatedAt = DateTimeOffset.FromUnixTimeSeconds(stat.st_ctime).UtcDateTime
                });
            }
        } while (currentDir != null);

        stats = entries.ToArray();

        Syscall.closedir(directoryFd);

        return UnixFsError.AutoHandle("close dir in internal read dir");
    }

    public UnixFsError? Rename(string oldPath, string newPath)
    {
        if (oldPath == newPath)
            return UnixFsError.NoError;

        var error = GetSafePath(oldPath, out int oldParentDirectoryFd, out string oldParentFileName,
            out Action oldCloseFd);

        if (error != null)
        {
            oldCloseFd.Invoke();
            return error;
        }

        if (oldParentFileName == ".")
            return UnixFsError.BuildFromErrno("Unable to rename root", 0);

        error = LStatAt(oldParentDirectoryFd, oldParentFileName, out _);

        if (error != null)

        {
            oldCloseFd.Invoke();
            return error;
        }

        error = GetSafePath(newPath, out int newParentDirectoryFd, out string newParentFileName,
            out Action newCloseFd);

        if (error != null)
        {
            newCloseFd.Invoke();

            if (error.Errno != Errno.ENOENT)
                return error;

            var parentFolderDir = Formatter.ReplaceEnd(newPath, newParentFileName, "").TrimEnd('/');
            error = MkdirAll(parentFolderDir, FilePermissions.ACCESSPERMS);

            if (error != null)
            {
                newCloseFd.Invoke();
                oldCloseFd.Invoke();
                return error;
            }

            error = GetSafePath(newPath, out newParentDirectoryFd, out newParentFileName,
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

        error = LStatAt(newParentDirectoryFd, newParentFileName, out _);

        if (error == null)
            return UnixFsError.BuildFromErrno("Target exists", Errno.EEXIST);
        else if (error.Errno != Errno.ENOENT)
            return error;

        Syscall.renameat(oldParentDirectoryFd, oldParentFileName, newParentDirectoryFd, newParentFileName);

        newCloseFd.Invoke();
        oldCloseFd.Invoke();

        return UnixFsError.AutoHandle("Error while renaming");
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

        var openAtResult = OpenAt(BaseDirectoryFd, parentDirectoryPath, OpenFlags.O_DIRECTORY | OpenFlags.O_RDONLY, 0,
            out int fd);
        parentDirectoryFd = fd;

        if (parentDirectoryFd != 0)
            closeFd = () => Syscall.close(fd);

        return openAtResult;
    }

    public UnixFsError? Touch(string path, OpenFlags flags, FilePermissions permissions, out FileStream fileStream)
    {
        if (!flags.HasFlag(OpenFlags.O_CREAT))
            flags |= OpenFlags.O_CREAT;

        int parentDirectoryFd;
        string fileName;
        Action closeFd;
        fileStream = default!;

        var error = GetSafePath(path, out parentDirectoryFd, out fileName, out closeFd);

        if (error != null)
        {
            closeFd.Invoke();

            var parentFolderDir = Formatter.ReplaceEnd(path, fileName, "").TrimEnd('/');
            error = MkdirAll(parentFolderDir, FilePermissions.ACCESSPERMS);

            if (error != null)
                return error;

            error = GetSafePath(path, out parentDirectoryFd, out fileName, out closeFd);

            if (error != null)
                return error;
        }

        error = OpenFileAt(parentDirectoryFd, fileName, flags, permissions, out SafeFileHandle handle);

        if (error != null)
            return error;

        fileStream = new FileStream(handle, FileAccess.ReadWrite);

        return UnixFsError.NoError;
    }

    public UnixFsError? Mkdir(string path, FilePermissions permissions)
    {
        var error = GetSafePath(path, out int parentDirectoryFd, out string fileName, out Action closeFd);

        if (error != null)
            return error;

        error = MkdirAt(parentDirectoryFd, fileName, permissions);

        closeFd.Invoke();

        return error;
    }

    public UnixFsError? MkdirAll(string path, FilePermissions permissions)
    {
        var normalized = NormalizePath(path);

        return Internal_MkdirAll(normalized, permissions);
    }

    public UnixFsError? Internal_MkdirAll(string path, FilePermissions permissions)
    {
        var error = LStat(path, out Stat stat);

        if (error == null)
        {
            if (IsFileType(stat.st_mode, FilePermissions.S_IFLNK))
            {
                error = Stat(path, out stat);

                if (error != null)
                    return error;
            }

            if (IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
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

            Internal_MkdirAll(parentDir, permissions);
        }

        error = Mkdir(path, permissions);

        if (error != null)
        {
            var lStatError = LStat(path, out Stat lstat);

            if (lStatError == null && IsFileType(lstat.st_mode, FilePermissions.S_IFDIR))
                return UnixFsError.NoError;

            return error;
        }

        return UnixFsError.NoError;
    }

    public UnixFsError? Stat(string path, out Stat stat)
    {
        return Internal_FStat(path, 0, out stat);
    }

    public UnixFsError? LStat(string path, out Stat stat)
    {
        return Internal_FStat(path, AtFlags.AT_SYMLINK_NOFOLLOW, out stat);
    }

    public UnixFsError? LStatAt(int parentDirectoryFd, string path, out Stat stat)
    {
        return Internal_FStatAt(parentDirectoryFd, path, AtFlags.AT_SYMLINK_NOFOLLOW, out stat);
    }

    public UnixFsError? Internal_FStat(string path, AtFlags flags, out Stat stat)
    {
        var error = GetSafePath(path, out int parentDirectoryFd, out string fileName, out Action closeFd);

        if (error != null)
        {
            stat = default;
            return error;
        }

        closeFd.Invoke();

        return Internal_FStatAt(parentDirectoryFd, fileName, flags, out stat);
    }

    public UnixFsError? Internal_FStatAt(int parentDirectoryFd, string name, AtFlags flags, out Stat stat)
    {
        Syscall.fstatat(parentDirectoryFd, name, out stat, flags);

        return UnixFsError.AutoHandle("An error occured in fstatat");
    }

    public UnixFsError? MkdirAt(int parentDirectoryFd, string path, FilePermissions permissions)
    {
        Syscall.mkdirat(parentDirectoryFd, path, permissions);

        return UnixFsError.AutoHandle("An error occured in mkdirat");
    }

    public UnixFsError? OpenFileAt(int parentDirectoryFd, string path, OpenFlags flags, FilePermissions permissions,
        out SafeFileHandle handle)
    {
        var error = OpenAt(parentDirectoryFd, path, flags, permissions, out int fileDescriptor);

        if (error != null)
        {
            handle = default!;
            return error;
        }

        handle = new SafeFileHandle(new IntPtr(fileDescriptor), true);
        return UnixFsError.NoError;
    }

    public UnixFsError? OpenAt(int parentDirectoryFd, string path, OpenFlags flags, FilePermissions permissions,
        out int fileDescriptor)
    {
        if (!flags.HasFlag(OpenFlags.O_NOFOLLOW))
            flags |= OpenFlags.O_NOFOLLOW;

        var openAtError = Internal_OpenAt(parentDirectoryFd, path, flags, permissions, out fileDescriptor);

        if (openAtError != null)
            return openAtError;

        // Validate file descriptor
        var finalPath = UnixPath.ReadLink("/proc/self/fd/" + fileDescriptor);

        var error = Stdlib.GetLastError();

        if (error != 0)
            return UnixFsError.BuildFromErrno($"Error reading symlink for fd {fileDescriptor}", error);

        if (!finalPath.StartsWith(BaseDirectory))
            throw new UnauthorizedAccessException(finalPath);

        return UnixFsError.NoError;
    }

    private UnixFsError? Internal_OpenAt(int parentDirectoryFd, string path, OpenFlags flags,
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

    private string NormalizePath(string input)
    {
        return input
            .Replace("//", "/")
            .Replace("..", "")
            .TrimStart('/');
    }

    private bool IsFileType(FilePermissions mode, FilePermissions type)
    {
        return (mode & FilePermissions.S_IFMT) == type;
    }
}