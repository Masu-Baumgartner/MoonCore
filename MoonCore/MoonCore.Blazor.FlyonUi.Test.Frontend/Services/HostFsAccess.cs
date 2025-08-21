using System.Net.Http.Json;
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

    public Task<string> GetFileUrl(string path)
        => Task.FromResult($"{Http.BaseAddress}api/download/file?path={path}");

    public Task<string> GetFolderUrl(string path)
        => Task.FromResult($"{Http.BaseAddress}api/download/folder?path={path}");

    public async Task Combine(string destination, string[] files)
    {
        await Http.PostAsync("api/fs/combine", JsonContent.Create(new FsCombineRequest()
        {
            Destination = destination,
            Files = files
        }));
    }

    public ArchiveFormat[] ArchiveFormats =>
    [
        new()
        {
            DisplayName = "ZIP Archive",
            Identifier = "zip",
            Extensions = ["zip"]
        },
        new()
        {
            DisplayName = "Tar-Gz Archive",
            Identifier = "tar.gz",
            Extensions = ["tar.gz"]
        }
    ];

    public async Task Archive(
        string path,
        ArchiveFormat format,
        string archiveRootPath,
        FsEntry[] files,
        Func<string, Task>? onProgress = null
    )
    {
        await Http.PostAsync("api/fs/compress", JsonContent.Create(new FsCompressRequest()
        {
            Destination = path,
            Identifier = format.Identifier,
            Files = files.Select(x => x.Name).ToArray(),
            Root = archiveRootPath
        }));
    }

    public Task Unarchive(string path, ArchiveFormat format, string archiveRootPath, Func<string, Task>? onProgress = null)
    {
        throw new NotImplementedException();
    }
}