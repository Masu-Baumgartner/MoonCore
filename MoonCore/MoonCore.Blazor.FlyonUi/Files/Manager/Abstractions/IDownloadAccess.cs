namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IDownloadUrlAccess
{
    public Task<string> GetFileUrl(string path);
    public Task<string> GetFolderUrl(string path);
}