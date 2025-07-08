namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public interface IFsAccess
{
    public Task CreateFile(string path);
    public Task CreateDirectory(string path);

    public Task<FsEntry[]> List(string path);
    public Task Move(string oldPath, string newPath);

    public Task Read(string path, Func<Stream, Task> onHandleData);
    public Task Write(string path, Stream dataStream);

    public Task Delete(string path);

    public Task UploadChunk(string path, int chunkId, long chunkSize, long totalSize, byte[] data);
    public Task<byte[]> DownloadChunk(string path, int chunkId, long chunkSize);
}