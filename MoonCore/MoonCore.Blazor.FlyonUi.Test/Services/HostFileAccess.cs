using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager;

namespace MoonCore.Blazor.FlyonUi.Test.Services;

public class HostFileAccess : IFileAccess
{
    private string RootDirectory;

    public HostFileAccess(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }

    public Task CreateFile(string path)
    {
        var fs = File.Create(
            Path.Combine(RootDirectory, path)
        );

        fs.Close();

        return Task.CompletedTask;
    }

    public Task CreateDirectory(string path)
    {
        Directory.CreateDirectory(
            Path.Combine(RootDirectory, path)
        );

        return Task.CompletedTask;
    }

    public Task<FileEntry[]> List(string path)
    {
        var entries = Directory.GetFileSystemEntries(
            Path.Combine(
                RootDirectory,
                path.TrimStart('/')
            )
        );

        var result = new List<FileEntry>();

        foreach (var entry in entries)
        {
            var fi = new FileInfo(entry);

            if (fi.Exists)
            {
                result.Add(new FileEntry()
                {
                    Name = fi.Name,
                    IsFolder = false,
                    Size = fi.Length,
                    CreatedAt = fi.CreationTimeUtc,
                    UpdatedAt = fi.LastWriteTimeUtc
                });
            }
            else
            {
                var di = new DirectoryInfo(entry);

                result.Add(new FileEntry()
                {
                    Name = di.Name,
                    IsFolder = true,
                    Size = 0,
                    CreatedAt = di.CreationTimeUtc,
                    UpdatedAt = di.LastWriteTimeUtc
                });
            }
        }

        return Task.FromResult(
            result.ToArray()
        );
    }

    public Task Move(string oldPath, string newPath)
    {
        if (File.Exists(oldPath))
            File.Move(oldPath, newPath);
        else
            Directory.Move(oldPath, newPath);

        return Task.CompletedTask;
    }

    public async Task Read(string path, Func<Stream, Task> onHandleData)
    {
        var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        await onHandleData.Invoke(fs);

        fs.Close();
    }

    public async Task Write(string path, Stream dataStream)
    {
        var fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

        await dataStream.CopyToAsync(fs);

        fs.Close();
    }

    public Task Delete(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
        else
            Directory.Delete(path, true);

        return Task.CompletedTask;
    }

    public Task Upload(Func<int, Task> updateProgress, string path, Stream dataStream)
    {
        return Task.CompletedTask;
    }

    public Task Download(Func<int, Task> updateProgress, string path, string fileName)
    {
        return Task.CompletedTask;
    }
}