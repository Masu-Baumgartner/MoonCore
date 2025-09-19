namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Defines a file system operation which can run on one and/or more files/folders
/// </summary>
public interface IMultiFsOperation : IFsOperation
{
    /// <summary>
    /// Css classes to use for the item in the context menu
    /// </summary>
    public string ContextCss { get; }
    
    /// <summary>
    /// Css classes to use for the toolbar button
    /// </summary>
    public string ToolbarCss { get; }

    /// <summary>
    /// Executes the operation for the specified files/folders which are located in the specified working directory
    /// </summary>
    /// <param name="workingDir">Working directory the <see cref="entries"/> are located in</param>
    /// <param name="entries">Files/folders the operation should run on</param>
    /// <param name="access">Currently used <see cref="IFsAccess"/></param>
    /// <param name="fileManager">Reference to the file manager itself</param>
    /// <returns></returns>
    public Task ExecuteAsync(string workingDir, FsEntry[] entries, IFsAccess access, IFileManager fileManager);
}