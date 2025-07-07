using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void OpenFileWrite(string path, Action<FileStream> handle, OpenFlags openFlags = OpenFlags.O_RDWR | OpenFlags.O_CREAT | OpenFlags.O_TRUNC)
    {
        OpenEntrySafe(path, (parentFd, fileName) =>
        {
            var fd = OpenAt(
                parentFd,
                fileName,
                openFlags,
                FilePermissions.ACCESSPERMS
            );

            try
            {
                var safeFileHandle = new SafeFileHandle(fd, true);
                var fs = new FileStream(safeFileHandle, FileAccess.ReadWrite);
                
                handle.Invoke(fs);
                
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