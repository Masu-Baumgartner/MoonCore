using Mono.Unix;
using Mono.Unix.Native;

namespace MoonCore.Helpers.Unix.Extensions;

public static class ReadDirExtensions
{
    public static UnixFsError? ReadDir(this UnixFileSystem fs, string path, out UnixFsEntry[] stats)
    {
        stats = Array.Empty<UnixFsEntry>();

        var error = fs.GetSafePath(path, out int parentDirectoryFd, out string fileName, out Action closeFd);

        if (error != null)
            return error;

        var openAtError = fs.OpenAt(parentDirectoryFd, fileName, OpenFlags.O_DIRECTORY | OpenFlags.O_RDONLY, 0,
            out int fileDescriptor);

        if (openAtError != null && fileName != "")
            return openAtError;

        error = fs.Internal_Readdir(fileName == "" ? parentDirectoryFd : fileDescriptor, out stats);

        closeFd.Invoke();
        
        if(openAtError == null && fileName != "")
            Syscall.close(fileDescriptor);

        return error;
    }

    public static UnixFsError? Internal_Readdir(this UnixFileSystem fs, int fd, out UnixFsEntry[] stats)
    {
        var directoryFd = Syscall.opendir(UnixPath.ReadLink("/proc/self/fd/" + fd));

        var lastError = Syscall.GetLastError();

        if (lastError != 0)
        {
            stats = Array.Empty<UnixFsEntry>();
            return UnixFsError.BuildFromErrno("fd open dir", lastError);
        }

        Dirent? currentDir;
        List<UnixFsEntry> entries = new();

        do
        {
            currentDir = Syscall.readdir(directoryFd);

            var readDirError = Syscall.GetLastError();

            if (readDirError != 0)
            {
                stats = Array.Empty<UnixFsEntry>();
                return UnixFsError.BuildFromErrno("readdir", readDirError);
            }

            if (currentDir != null)
            {
                if (currentDir.d_name == "." || currentDir.d_name == "..")
                    continue;

                var error = fs.LStatAt(fd, currentDir.d_name, out var stat);

                if (error != null)
                {
                    stats = Array.Empty<UnixFsEntry>();
                    return error;
                }

                entries.Add(new()
                {
                    Name = currentDir.d_name,
                    IsDirectory = fs.IsFileType(stat.st_mode, FilePermissions.S_IFDIR),
                    IsFile = fs.IsFileType(stat.st_mode, FilePermissions.S_IFREG),
                    Size = stat.st_size,
                    LastChanged = DateTimeOffset.FromUnixTimeSeconds(stat.st_mtime).UtcDateTime,
                    CreatedAt = DateTimeOffset.FromUnixTimeSeconds(stat.st_ctime).UtcDateTime
                });
            }
        } while (currentDir != null);

        stats = entries.ToArray();

        Syscall.closedir(directoryFd);

        return UnixFsError.AutoHandle("close dir in internal read dir");
    }
}