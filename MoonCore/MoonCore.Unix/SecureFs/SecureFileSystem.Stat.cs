using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
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