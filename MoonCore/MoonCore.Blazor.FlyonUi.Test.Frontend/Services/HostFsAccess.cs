using System.Net.Http.Json;
using System.Text.Encodings.Web;
using MoonCore.Blazor.FlyonUi.Files;
using MoonCore.Blazor.FlyonUi.Files.Manager;
using MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Request;

namespace MoonCore.Blazor.FlyonUi.Test.Frontend.Services;

public class HostFsAccess : IFsAccess, IDownloadUrlAccess, ICombineAccess, IArchiveAccess
{
    private readonly HttpClient Http;

    public HostFsAccess(HttpClient http)
    {
        Http = http;
    }

    public Task CreateFileAsync(string path)
        => Http.PostAsync($"api/fs/create-file?path={path}", null);

    public Task CreateDirectoryAsync(string path)
        => Http.PostAsync($"api/fs/create-directory?path={path}", null);

    public async Task<FsEntry[]> ListAsync(string path)
        => await Http.GetFromJsonAsync<FsEntry[]>($"api/fs/list?path={path}") ?? [];

    public Task MoveAsync(string oldPath, string newPath)
        => Http.PostAsync($"api/fs/move?oldPath={oldPath}&newPath={newPath}", null);

    public async Task ReadAsync(string path, Func<Stream, Task> onHandleData)
    {
        var encoded = Uri.EscapeDataString(path.TrimStart('/'));
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/fs/download?path={encoded}");
        using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        await onHandleData(stream);
    }

    public async Task DeleteAsync(string path)
    {
        var encodedPath = UrlEncoder.Default.Encode(path);
        await Http.DeleteAsync($"api/fs/delete?path={encodedPath}");
    }

    public async Task WriteAsync(string path, Stream dataStream)
    {
        using var content = new MultipartFormDataContent
        {
            { new StreamContent(dataStream), "file", "file" }
        };
        await Http.PostAsync($"api/fs/upload/single?filePath={path.TrimStart('/')}", content);
    }

    public Task<string> GetFileUrlAsync(string path)
        => Task.FromResult($"{Http.BaseAddress}api/download/file?path={path}");

    public Task<string> GetFolderUrlAsync(string path)
        => Task.FromResult($"{Http.BaseAddress}api/download/folder?path={path}");

    public async Task CombineAsync(string destination, string[] files)
    {
        await Http.PostAsync("api/fs/combine", JsonContent.Create(new FsCombineRequest()
        {
            Destination = destination,
            Files = files
        }));
    }

    public ArchiveFormat[] ArchiveFormats =>
    [
        new(
            displayName: "ZIP Archive",
            identifier: "zip",
            extensions: ["zip"]
        ),
        new(
            displayName: "Tar-Gz Archive",
            identifier: "tar.gz",
            extensions: ["tar.gz"]
        )
    ];

    public async Task ArchiveAsync(
        string destination,
        ArchiveFormat format,
        string archiveRootPath,
        FsEntry[] files,
        Func<string, Task>? onProgress = null
    )
    {
        await Http.PostAsync("api/fs/compress", JsonContent.Create(new FsCompressRequest()
        {
            Destination = destination,
            Identifier = format.Identifier,
            Files = files.Select(x => x.Name).ToArray(),
            Root = archiveRootPath
        }));
    }

    public Task UnarchiveAsync(string path, ArchiveFormat format, string archiveRootPath,
        Func<string, Task>? onProgress = null)
    {
        throw new NotImplementedException();
    }
}