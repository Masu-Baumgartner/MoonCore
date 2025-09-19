namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Defines functionality a <see cref="IFsAccess"/> needs to implement in order for mooncore to download files
/// and folders using <see cref="GetFileUrlAsync"/> and <see cref="GetFolderUrlAsync"/>
/// </summary>
public interface IDownloadUrlAccess
{
    /// <summary>
    /// Retries the download url for the specified file
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <returns>Download URL</returns>
    public Task<string> GetFileUrlAsync(string path);
    
    /// <summary>
    /// Retries the download url for the specified folder
    /// This should return a zip archive or similar so the browser can actually download it
    /// </summary>
    /// <param name="path">Path of the folder</param>
    /// <returns>Download URL</returns>
    public Task<string> GetFolderUrlAsync(string path);
}