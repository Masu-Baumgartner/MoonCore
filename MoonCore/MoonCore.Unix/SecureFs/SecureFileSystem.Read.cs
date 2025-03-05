using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public void OpenFileRead(string path, Action<FileStream> handle)
    {
        OpenEntrySafe(path, (parentFd, fileName) =>
        {
            var fd = OpenAt(
                parentFd,
                fileName,
                OpenFlags.O_RDONLY,
                FilePermissions.ACCESSPERMS
            );

            try
            {
                var safeFileHandle = new SafeFileHandle(fd, true);
                var fs = new FileStream(safeFileHandle, FileAccess.Read);
                
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