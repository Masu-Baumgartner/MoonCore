using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void WriteFile(string path, Stream dataStream)
    {
        OpenEntrySafe(path, (parentFd, fileName) =>
        {
            var fd = OpenAt(
                parentFd,
                fileName,
                OpenFlags.O_RDWR | OpenFlags.O_CREAT | OpenFlags.O_TRUNC,
                FilePermissions.ACCESSPERMS
            );
            
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
            
            try
            {
                var safeFileHandle = new SafeFileHandle(fd, true);
                var fs = new FileStream(safeFileHandle, FileAccess.Write);

                dataStream.CopyToAsync(fs);
                
                fs.Flush();
                fs.Close();
            }
            catch (Exception)
            {
                Syscall.close(fd);
                throw;
            }
        });
    }
}