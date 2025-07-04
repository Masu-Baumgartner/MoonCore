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
        HttpClient httpClient)
    {
        RootDirectory = rootDirectory;
        ChunkedFileTransferService = chunkedFileTransferService;
        HttpClient = httpClient;
    }

    private string HandleRawPath(string path)
        => Path.Combine(RootDirectory, path.TrimStart('/'));

    public Task CreateFile(string path)
    {
        var fs = File.Create(
            HandleRawPath(path)
        );

        fs.Close();

        return Task.CompletedTask;
    }

    public Task CreateDirectory(string path)
    {
        Directory.CreateDirectory(
            HandleRawPath(path)
        );

        return Task.CompletedTask;
    }

    public Task<FileEntry[]> List(string path)
    {
        var entries = Directory.GetFileSystemEntries(
            HandleRawPath(path)
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
       var oldPathSafe = HandleRawPath(oldPath);
       var newPathSafe = HandleRawPath(newPath);

        if (File.Exists(oldPathSafe))
            File.Move(oldPathSafe, newPathSafe);
        else
            Directory.Move(oldPathSafe, newPathSafe);

        return Task.CompletedTask;
    }

    public async Task Read(string path, Func<Stream, Task> onHandleData)
    {
        var fs = File.Open(HandleRawPath(path), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        try
        {
            await onHandleData.Invoke(fs);
        }
        finally
        {
            fs.Close();
        }
    }

    public async Task Write(string path, Stream dataStream)
    {
        var fs = File.Open(HandleRawPath(path), FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

        try
        {
            await dataStream.CopyToAsync(fs);
        }
        finally
        {
            fs.Close();
        }
    }

    public Task Delete(string path)
    {
        var pathSafe = HandleRawPath(path);

        if (File.Exists(pathSafe))
            File.Delete(pathSafe);
        else
            Directory.Delete(pathSafe, true);

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