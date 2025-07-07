using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void Rename(string oldPath, string newPath)
    {
        if (oldPath == newPath)
            return;

        // Ensure old target exists
        SecureLStat(oldPath);
        
        // Ensure new path doesnt exist
        try
        {
            SecureLStat(newPath);

            throw new ArgumentException("The element on the new path already exists");
        }
        catch (SyscallException syscallException)
        {
            // Return all errors except the one we expect
            if (syscallException.Errno != Errno.ENOENT)
                throw;

            // Ensure target parent exists
            var newParentDir = Path.GetDirectoryName(newPath) ?? "";
            MkdirAll(newParentDir, FilePermissions.ACCESSPERMS);
            
            // Open old entry
            OpenEntrySafe(oldPath, (oldParentFd, oldName) =>
            {
                // Open new entry
                OpenEntrySafe(newPath, (newParentFd, newName) =>
                {
                    // Now we can rename
                    Syscall.renameat(oldParentFd, oldName, newParentFd, newName);
                    
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
                                throw new SyscallException(lastError, "Symlink detected in path while symlink following has been disabled");
                
                            default:
                                throw new SyscallException(lastError);
                        }
                    }
                });
            });
        }
    }
}