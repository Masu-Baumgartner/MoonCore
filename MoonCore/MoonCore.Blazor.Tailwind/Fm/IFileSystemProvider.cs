namespace MoonCore.Blazor.Tailwind.Fm;

public interface IFileSystemProvider
{
    public Task<FileSystemEntry[]> List(string path);
    public Task Create(string path, Stream stream);
    public Task Move(string oldPath, string newPath);
    public Task Delete(string path);
    public Task CreateDirectory(string path);
    public Task<Stream> Read(string path);
    public Task Download(Func<int, Task> updateProgress, string path, string fileName);
    public Task Upload(Func<int, Task> updateProgress, string path, Stream stream);
}