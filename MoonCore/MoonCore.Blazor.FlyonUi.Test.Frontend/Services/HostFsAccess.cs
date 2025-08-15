using System.Net.Http.Json;
using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

namespace MoonCore.Blazor.FlyonUi.Test.Frontend.Services;

public class HostFsAccess : IFsAccess, IDownloadUrlAccess
{
    private readonly HttpClient Http;

    public HostFsAccess(HttpClient http)
    {
        Http = http;
    }

    public Task CreateFile(string path)
        => Http.PostAsync($"api/fs/create-file?path={path}", null);

    public Task CreateDirectory(string path)
        => Http.PostAsync($"api/fs/create-directory?path={path}", null);

    public async Task<FsEntry[]> List(string path)
        => await Http.GetFromJsonAsync<FsEntry[]>($"api/fs/list?path={path}") ?? [];

    public Task Move(string oldPath, string newPath)
        => Http.PostAsync($"api/fs/move?oldPath={oldPath}&newPath={newPath}", null);

    public async Task Read(string path, Func<Stream, Task> onHandleData)
    {
        var encoded = Uri.EscapeDataString(path.TrimStart('/'));
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/fs/download?path={encoded}");
        using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        await onHandleData(stream);
    }

    public Task Delete(string path)
        => Http.DeleteAsync($"api/fs/delete?path={path}");

    public async Task Write(string path, Stream dataStream)
    {
        using var content = new MultipartFormDataContent
        {
            { new StreamContent(dataStream), "file", "file" }
        };
        await Http.PostAsync($"api/fs/upload/single?filePath={path.TrimStart('/')}", content);
    }

    public async Task UploadChunk(string path, int chunkId, long chunkSize, long totalSize, byte[] data)
    {
        using var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(data), "file", "file" }
        };
        await Http.PostAsync(
            $"api/fs/upload?chunkId={chunkId}&chunkSize={chunkSize}&fileSize={totalSize}&fileName={path.TrimStart('/')}",
            content
        );
    }

    public Task<byte[]> DownloadChunk(string path, int chunkId, long chunkSize)
        => Http.GetByteArrayAsync(
            $"api/fs/download-chunk?chunkId={chunkId}&chunkSize={chunkSize}&path={path.TrimStart('/')}");

    public Task<string> GetFileUrl(string path)
        => Task.FromResult($"{Http.BaseAddress}api/download/file?path={path}");

    public Task<string> GetFolderUrl(string path)
        => Task.FromResult($"{Http.BaseAddress}api/download/folder?path={path}");
}