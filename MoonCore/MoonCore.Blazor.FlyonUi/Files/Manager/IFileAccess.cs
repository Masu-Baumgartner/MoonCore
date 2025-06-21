namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public interface IFileAccess
{
    public Task CreateFile(string path);
    public Task CreateDirectory(string path);

    public Task<FileEntry[]> List(string path);
    public Task Move(string oldPath, string newPath);

    public Task Read(string path, Func<Stream, Task> onHandleData);
    public Task Write(string path, Stream dataStream);

    public Task Delete(string path);

    public Task Upload(Func<int, Task> updateProgress, string path, Stream dataStream);
    public Task Download(Func<int, Task> updateProgress, string path, FileEntry fileEntry);
}