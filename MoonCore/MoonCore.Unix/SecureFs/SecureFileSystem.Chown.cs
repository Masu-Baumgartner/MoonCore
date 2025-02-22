using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void Chown(string path, int userId, int groupId)
    {
        OpenEntrySafe(path, (parentFd, name) =>
        {
            if (string.IsNullOrEmpty(name) || name == ".")
                throw new ArgumentException("Unable to chown root path");

            Syscall.fchownat(parentFd, name, userId, groupId, AtFlags.AT_SYMLINK_NOFOLLOW);
            
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
                    
                    default:
                        throw new SyscallException(lastError);
                }
            }
        });
    }

    public void ChownAll(string path, int userId, int groupId)
    {
        var stat = SecureLStat(path);
        
        // Not a dir => chown and return
        if (!IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
        {
            Chown(path, userId, groupId);
            return;
        }

        var entries = ReadDir(path);

        foreach (var entry in entries)
        {
            // Skip files which already have the correct permission
            if(entry.OwnerUserId == userId && entry.OwnerGroupId == groupId && entry.IsFile)
                continue;
            
            var entryPath = $"{path}/{entry.Name}";
            ChownAll(entryPath, userId, groupId);
        }
        
        // Finally chown our self
        Chown(path, userId, groupId);
    }
}