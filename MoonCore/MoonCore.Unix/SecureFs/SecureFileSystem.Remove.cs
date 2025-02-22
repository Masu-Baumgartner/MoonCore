using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void Remove(string path)
    {
        OpenEntrySafe(path, (parentFd, name) =>
        {
            if (name == ".")
                throw new ArgumentException("You cannot remove the root path");

            // Try unlinking file
            Syscall.unlinkat(parentFd, name, 0);
            var unlinkError = Stdlib.GetLastError();
            
            // If unlinking the file was successful, we can exit here
            if(unlinkError == 0)
                return;

            // Try unlinking folder
            Syscall.unlinkat(parentFd, name, AtFlags.AT_REMOVEDIR);
            var dirUnlinkError = Stdlib.GetLastError();
            
            // If unlinking the folder was successful, we can exit here
            if(dirUnlinkError == 0)
                return;

            // Handle dir specific unlink errors
            if (dirUnlinkError != Errno.ENOENT)
                throw new SyscallException(dirUnlinkError);

            throw new SyscallException(unlinkError);
        });
    }

    public void RemoveAll(string path)
    {
        // Prevent invalid values
        if (string.IsNullOrEmpty(path))
            return;

        if (path.EndsWith("."))
            throw new ArgumentException("Unable to remove root path");

        // Try to remove it as it is a regular entry
        try
        {
            Remove(path);
            
            // If it doesn't throw an exception, we can return as successful
            return;
        }
        catch (SyscallException e)
        {
            // If it doesn't exist we can return as successful
            if(e.Errno == Errno.ENOENT)
                return;
            
            // If the rm failed, we need to check how we can rm it so we lstat
            var stat = SecureLStat(path);
            
            // Not a directory? we do not how to remove the entry, so we return the initial rm error
            if (!IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
                throw;
            
            // Read the contents of the directory, so whe know what to remove
            var entries = ReadDir(path);

            // If we didn't fail because there were items in the dir, we can throw the initial error
            if (entries.Length == 0)
                throw;

            // Delete recursively and stop if any error has been encountered
            foreach (var entry in entries)
            {
                var entryPath = $"{path}/{entry.Name}";
                RemoveAll(entryPath);
            }
            
            // Now that the contents of the directory are deleted, we can try to delete this item we initially wanted to delete
            try
            {
                Remove(path);
                
                // If it doesn't throw an exception, we can return as successful
                return;
            }
            catch (SyscallException syscallException)
            {
                // If it doesn't exist we can return as successful
                if(syscallException.Errno == Errno.ENOENT)
                    return;

                throw;
            }
        }
    }
}