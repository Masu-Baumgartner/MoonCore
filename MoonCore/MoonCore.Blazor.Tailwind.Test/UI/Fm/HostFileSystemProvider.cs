using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public class HostFileSystemProvider : IFileSystemProvider
{
    private readonly string BaseDirectory;

    public HostFileSystemProvider(string baseDirectory)
    {
        BaseDirectory = baseDirectory;
    }

    public async Task<FileSystemEntry[]> List(string path)
    {
        var entries = new List<FileSystemEntry>();
        
        var files = Directory.GetFiles(PathBuilder.Dir(BaseDirectory, path));

        foreach (var file in files)
        {
            var fi = new FileInfo(file);
            
            entries.Add(new FileSystemEntry()
            {
                Name = fi.Name,
                Size = fi.Length,
                CreatedAt = fi.CreationTimeUtc,
                IsFile = true,
                UpdatedAt = fi.LastWriteTimeUtc
            });
        }

        var directories = Directory.GetDirectories(PathBuilder.Dir(BaseDirectory, path));

        foreach (var directory in directories)
        {
            var di = new DirectoryInfo(directory);
            
            entries.Add(new FileSystemEntry()
            {
                Name = di.Name,
                Size = 0,
                CreatedAt = di.CreationTimeUtc,
                UpdatedAt = di.LastWriteTimeUtc,
                IsFile = false
            });
        }

        return entries.ToArray();
    }

    public Task Create(string path, Stream stream)
    {
        throw new NotImplementedException();
    }

    public Task Move(string oldPath, string newPath)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string path)
    {
        throw new NotImplementedException();
    }

    public Task CreateDirectory(string path)
    {
        throw new NotImplementedException();
    }
}