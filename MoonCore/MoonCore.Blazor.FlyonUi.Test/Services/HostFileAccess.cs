using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Test.Services;

public class HostFileAccess : IFileAccess
{
    private readonly string RootDirectory;
    private readonly ChunkedFileTransferService ChunkedFileTransferService;
    private readonly long ChunkSize = ByteConverter.FromMegaBytes(20).Bytes;
    private readonly HttpClient HttpClient;

    public HostFileAccess(string rootDirectory, ChunkedFileTransferService chunkedFileTransferService,
        HttpClient httpClientr)
    {
        RootDirectory = rootDirectory;
        ChunkedFileTransferService = chunkedFileTransferService;
        HttpClient = httpClientr;
    }

    public Task CreateFile(string path)
    {
        path = path.TrimStart('/');

        var fs = File.Create(
            Path.Combine(RootDirectory, path)
        );

        fs.Close();

        return Task.CompletedTask;
    }

    public Task CreateDirectory(string path)
    {
        path = path.TrimStart('/');

        Directory.CreateDirectory(
            Path.Combine(RootDirectory, path)
        );

        return Task.CompletedTask;
    }

    public Task<FileEntry[]> List(string path)
    {
        path = path.TrimStart('/');

        var entries = Directory.GetFileSystemEntries(
            Path.Combine(
                RootDirectory,
                path
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
        oldPath = oldPath.TrimStart('/');
        newPath = newPath.TrimStart('/');

        if (File.Exists(oldPath))
            File.Move(oldPath, newPath);
        else
            Directory.Move(oldPath, newPath);

        return Task.CompletedTask;
    }

    public async Task Read(string path, Func<Stream, Task> onHandleData)
    {
        path = path.TrimStart('/');

        var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        await onHandleData.Invoke(fs);

        fs.Close();
    }

    public async Task Write(string path, Stream dataStream)
    {
        path = path.TrimStart('/');

        var fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

        await dataStream.CopyToAsync(fs);

        fs.Close();
    }

    public Task Delete(string path)
    {
        path = path.TrimStart('/');

        if (File.Exists(path))
            File.Delete(path);
        else
            Directory.Delete(path, true);

        return Task.CompletedTask;
    }

    public async Task Upload(string path, Stream dataStream, Func<int, Task> updateProgress)
    {
        path = path.TrimStart('/');

        await ChunkedFileTransferService.Upload(
            dataStream,
            ChunkSize,
            async (chunkId, content) =>
            {
                var multipartForm = new MultipartFormDataContent();
                multipartForm.Add(content, "file", "file");

                await HttpClient.PostAsync(
                    $"http://localhost:5220/api/upload" +
                    $"?chunkId={chunkId}" +
                    $"&chunkSize={ChunkSize}" +
                    $"&fileSize={dataStream.Length}" +
                    $"&fileName={path}",
                    multipartForm
                );
            },
            new Progress<int>(async void (percent) => { await updateProgress.Invoke(percent); })
        );
    }

    public Task Download(string path, FileEntry fileEntry, Func<int, Task> updateProgress)
    {
        path = path.TrimStart('/');

        throw new NotImplementedException();
    }
}