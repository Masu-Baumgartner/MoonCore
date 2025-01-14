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

    public async Task Create(string path, Stream stream)
    {
        var baseDir = Path.GetDirectoryName(path);
        
        if(!string.IsNullOrEmpty(baseDir))
            Directory.CreateDirectory(PathBuilder.Dir(BaseDirectory, baseDir));

        await using var fs = File.Create(PathBuilder.File(BaseDirectory, path));
        
        await stream.CopyToAsync(fs);
        
        await fs.FlushAsync();
        fs.Close();
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
        Directory.CreateDirectory(PathBuilder.Dir(BaseDirectory, path));
        return Task.CompletedTask;
    }
}