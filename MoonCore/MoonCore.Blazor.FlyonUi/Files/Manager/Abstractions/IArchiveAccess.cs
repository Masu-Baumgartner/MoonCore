namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Defines the functionally a <see cref="IFsAccess"/> needs to support in order for archiving features being
/// available in the file manager
/// </summary>
public interface IArchiveAccess
{
    /// <summary>
    /// Array of supported archive formats
    /// </summary>
    public ArchiveFormat[] ArchiveFormats { get; }

    /// <summary>
    /// Archive one or more files from a specific root path to a specific destination
    /// </summary>
    /// <param name="destination">Destination path where the archive should be created at. Needs to be a file path</param>
    /// <param name="format">Selected format for the archive</param>
    /// <param name="archiveRootPath">Root path of the archive</param>
    /// <param name="files">Array of files/folders to archive</param>
    /// <param name="onProgress">Optional callback for progress updates</param>
    /// <returns></returns>
    public Task ArchiveAsync(string destination, ArchiveFormat format, string archiveRootPath, FsEntry[] files,
        Func<string, Task>? onProgress = null);

    // TODO: Document and implement
    public Task UnarchiveAsync(string path, ArchiveFormat format, string archiveRootPath,
        Func<string, Task>? onProgress = null);
}