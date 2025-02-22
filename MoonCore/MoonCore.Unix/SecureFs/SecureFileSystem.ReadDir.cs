using Mono.Unix;
using Mono.Unix.Native;
using MoonCore.Unix.Exceptions;

namespace MoonCore.Unix.SecureFs;

public partial class SecureFileSystem
{
    public SecureFsEntry[] ReadDir(string path)
    {
        //Console.WriteLine("Read dir: " + path);
        
        List<SecureFsEntry> entries = [];

        OpenEntrySafe(path, (parentFd, dirName) =>
        {
            int dirFd;

            // If we want to open the root directory, we are using the already opened fd for the root dir
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(dirName))
                dirFd = parentFd;
            else
            {
                dirFd = OpenAt(
                    parentFd,
                    dirName,
                    OpenFlags.O_DIRECTORY | OpenFlags.O_NOFOLLOW | OpenFlags.O_RDONLY,
                    0
                );
            }

            // Handle possible errors
            Errno lastError;
            
            try
            {
                lastError = Stdlib.GetLastError();
            }
            catch (ArgumentOutOfRangeException)
            {
                // We got a 203 when reading the dir while chown-ing,
                // which the commented out console write line fixed somehow. i chose to ignore the error fow now
                lastError = 0;
            }

            if (lastError != 0)
            {
                switch (lastError)
                {
                    case Errno.ENOENT:
                        throw new SyscallException(lastError, "No file or directory found using the provided path");

                    case Errno.EBADF:
                        throw new SyscallException(lastError, "Bad parent file descriptor");

                    case Errno.ELOOP:
                        throw new SyscallException(lastError,
                            "Symlink detected in path while symlink following has been disabled");

                    default:
                        throw new SyscallException(lastError);
                }
            }

            // Read directory entries
            Dirent? currentDirent;

            // We want to open a dir pointer instead of using the original fd because
            // if we would do this on the root fd, it would lead to the dir stream to be empty on second read
            var dirPointer = Syscall.opendir(
                UnixPath.ReadLink($"/proc/self/fd/{dirFd}")
            );

            do
            {
                currentDirent = Syscall.readdir(dirPointer);

                // Handle possible errors
                lastError = Stdlib.GetLastError();

                if (lastError != 0)
                {
                    switch (lastError)
                    {
                        case Errno.ENOENT:
                            throw new SyscallException(lastError, "No file or directory found using the provided path");

                        case Errno.EBADF:
                            throw new SyscallException(lastError, "Bad parent file descriptor");

                        case Errno.ELOOP:
                            throw new SyscallException(lastError,
                                "Symlink detected in path while symlink following has been disabled");

                        default:
                            throw new SyscallException(lastError);
                    }
                }

                // No entry found? let's exit the loop
                if (currentDirent == null)
                    continue;

                // Don't look at the always existing symlinks to the parent dir
                if (currentDirent.d_name == "." || currentDirent.d_name == "..")
                    continue;

                // Stat the current entry
                var stat = SecureLStatAt(dirFd, currentDirent.d_name);

                // Convert stat to entry and add it to list
                entries.Add(new SecureFsEntry()
                {
                    Name = currentDirent.d_name,
                    IsDirectory = IsFileType(stat.st_mode, FilePermissions.S_IFDIR),
                    IsFile = IsFileType(stat.st_mode, FilePermissions.S_IFREG),
                    Size = stat.st_size,
                    LastChanged = DateTimeOffset.FromUnixTimeSeconds(stat.st_mtime).UtcDateTime,
                    CreatedAt = DateTimeOffset.FromUnixTimeSeconds(stat.st_ctime).UtcDateTime,
                    OwnerUserId = (int)stat.st_uid,
                    OwnerGroupId = (int)stat.st_gid
                });
            } while (currentDirent != null);
            
            // Close dir pointer
            Syscall.closedir(dirPointer);

            // We only want to close newly opened fds
            if (dirFd != RootFileDescriptor)
                Syscall.close(dirFd);
        });

        return entries.ToArray();
    }
}