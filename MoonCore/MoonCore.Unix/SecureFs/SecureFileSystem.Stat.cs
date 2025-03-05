using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public SecureFsEntry Stat(string path)
    {
        var stat = SecureStat(path);

        return new SecureFsEntry()
        {
            Name = Path.GetFileName(path),
            IsDirectory = IsFileType(stat.st_mode, FilePermissions.S_IFDIR),
            IsFile = IsFileType(stat.st_mode, FilePermissions.S_IFREG),
            Size = stat.st_size,
            LastChanged = DateTimeOffset.FromUnixTimeSeconds(stat.st_mtime).UtcDateTime,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(stat.st_ctime).UtcDateTime,
            OwnerUserId = (int)stat.st_uid,
            OwnerGroupId = (int)stat.st_gid
        };
    }
    
    private Stat SecureStat(string path)
        => SecureFStat(path, AtFlags.AT_SYMLINK_NOFOLLOW);
    
    private Stat SecureLStat(string path)
        => SecureFStat(path, AtFlags.AT_SYMLINK_NOFOLLOW);
    
    private Stat SecureLStatAt(int parentFd, string dirName)
        => SecureFStatAt(parentFd, dirName, AtFlags.AT_SYMLINK_NOFOLLOW);
    
    private Stat SecureFStat(string path, AtFlags atFlags)
    {
        Stat result = default;
        
        OpenEntrySafe(path, (parentFd, dirName) =>
        {
            result = SecureFStatAt(parentFd, dirName, atFlags);
        });

        return result;
    }

    private Stat SecureFStatAt(int parentFd, string name, AtFlags atFlags)
    {
        var err = Syscall.fstatat(parentFd, name, out var stat, atFlags);

        if (err == 0)
            return stat;
        
        // Handle possible errors
        var error = Stdlib.GetLastError();
        
        switch (error)
        {
            case Errno.ENOENT:
                throw new SyscallException(error, "No file or directory found using the provided path");

            case Errno.EBADF:
                throw new SyscallException(error, "Bad parent file descriptor");
            
            default:
                throw new SyscallException(error);
        }
    }
}