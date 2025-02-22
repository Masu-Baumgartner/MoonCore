using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void Mkdir(string path, FilePermissions filePermissions)
    {
        // We expect the root path to be always created, so we can return here
        if(string.IsNullOrEmpty(path) || path == ".")
            return;
        
        OpenEntrySafe(path, (parentFd, dirName) =>
        {
            Syscall.mkdirat(parentFd, dirName, filePermissions);
            
            // Handle possible errors
            var lastError = Stdlib.GetLastError();

            if (lastError != 0)
            {
                switch (lastError)
                {
                    case Errno.ENOENT:
                        throw new SyscallException(lastError, "No file or directory found using the provided path");
                    
                    case Errno.EEXIST:
                        throw new SyscallException(lastError, "The directory does already exist");
                
                    case Errno.EBADF:
                        throw new SyscallException(lastError, "Bad parent file descriptor");
                
                    default:
                        throw new SyscallException(lastError);
                }
            }
        });
    }

    public void MkdirAll(string path, FilePermissions filePermissions)
    {
        // We expect the root path to be always created, so we can return here
        if(string.IsNullOrEmpty(path) || path == ".")
            return;
        
        try
        {
            var stat = SecureLStat(path);

            // Resolve link
            if (IsFileType(stat.st_mode, FilePermissions.S_IFLNK))
                stat = SecureStat(path);

            // Exit if it already exists
            if(IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
                return;
        }
        catch (SyscallException e)
        {
            // Filter out all no "missing errors"
            if (e.Errno != Errno.ENOENT)
                throw;
            
            // Handle recursion
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

                MkdirAll(parentDir, filePermissions);
            }

            // Actually creating

            try
            {
                Mkdir(path, filePermissions);
            }
            catch (SyscallException mkdirException)
            {
                // If we encounter any errors, we want to check if we created the directory never the less
                
                try
                {
                    var lStat = SecureLStat(path);

                    if (IsFileType(lStat.st_mode, FilePermissions.S_IFDIR))
                        return;
                }
                catch (Exception)
                {
                    // If the lstat fails, we want to return the original error
                    throw mkdirException;
                }
            }
        }
    }
}