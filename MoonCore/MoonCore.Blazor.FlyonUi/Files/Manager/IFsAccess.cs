namespace MoonCore.Blazor.FlyonUi.Files.Manager;

public interface IFsAccess
{
    /// <summary>
    /// Creates a new and empty file at the specified path
    /// </summary>
    /// <param name="path">File path to create the file at</param>
    /// <returns></returns>
    public Task CreateFileAsync(string path);
    
    /// <summary>
    /// Creates a new directory at the specified path
    /// </summary>
    /// <param name="path">Path to create the directory at</param>
    /// <returns></returns>
    public Task CreateDirectoryAsync(string path);

    /// <summary>
    /// Lists all folders and files at a specific path
    /// </summary>
    /// <param name="path">Path to search for files and folders</param>
    /// <returns></returns>
    public Task<FsEntry[]> ListAsync(string path);
    
    /// <summary>
    /// Moves a file/folder from <see cref="oldPath"/> to <see cref="newPath"/>
    /// </summary>
    /// <param name="oldPath">Current path of the item</param>
    /// <param name="newPath">New path of the item</param>
    /// <returns></returns>
    public Task MoveAsync(string oldPath, string newPath);

    /// <summary>
    /// Reads the specified file and provides a callback which contains an automatically disposing stream of the data
    /// </summary>
    /// <param name="path">File path to read from</param>
    /// <param name="onHandleData">Callback providing the data stream. After the callback returns the stream is disposed</param>
    /// <returns></returns>
    public Task ReadAsync(string path, Func<Stream, Task> onHandleData);
    
    /// <summary>
    /// Writes the specified stream to the file path. This creates a new file if missing
    /// </summary>
    /// <param name="path">File path to write to</param>
    /// <param name="dataStream">Stream containing the data which should be written</param>
    /// <returns></returns>
    public Task WriteAsync(string path, Stream dataStream);

    /// <summary>
    /// Deletes a folder/file at the provided path
    /// </summary>
    /// <param name="path">Path of the item to delete</param>
    /// <returns></returns>
    public Task DeleteAsync(string path);
}