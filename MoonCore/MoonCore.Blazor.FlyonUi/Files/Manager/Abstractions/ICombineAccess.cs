namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Defines the functionality a <see cref="IFsAccess"/> needs to implement in order for the file manager to
/// be ale to use the compress feature.
/// This is used for chunked uploading. The file manager will create a tmp directory,
/// upload the large file in multiple parts as regular upload/write operations and then
/// combine them using the <see cref="CombineAsync"/> operation and remove the temporary folder
/// </summary>
public interface ICombineAccess
{
    /// <summary>
    /// Combines multiple files into one. The order from the files specified in <see cref="files"/> is respected
    /// </summary>
    /// <param name="destination">Destination of the combined file</param>
    /// <param name="files">Array of files to combine. Absolute paths from the file manager root</param>
    /// <returns></returns>
    public Task CombineAsync(string destination, string[] files);
}