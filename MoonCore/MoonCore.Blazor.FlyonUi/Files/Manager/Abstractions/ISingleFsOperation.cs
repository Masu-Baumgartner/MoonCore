namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Defines a file system operation which can run only on one file which matches a specific <see cref="Filter"/>
/// </summary>
public interface ISingleFsOperation : IFsOperation
{
    /// <summary>
    /// Css classes for the context menu entry
    /// </summary>
    public string ContextCss { get; }
    
    /// <summary>
    /// Determines if the operation should be available for the specified file. True if it is compatible otherwise false
    /// </summary>
    public Func<FsEntry, bool>? Filter { get; }

    /// <summary>
    /// Runs the operation on the file/folder located in the working directory
    /// </summary>
    /// <param name="workingDir">Directory the <see cref="entry"/> is located in</param>
    /// <param name="entry">File/folder the operation should run on</param>
    /// <param name="access">Currently used <see cref="IFsAccess"/></param>
    /// <param name="fileManager">Reference to the file manager itself</param>
    /// <returns></returns>
    public Task ExecuteAsync(string workingDir, FsEntry entry, IFsAccess access, IFileManager fileManager);
}