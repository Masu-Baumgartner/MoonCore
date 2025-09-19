using Microsoft.AspNetCore.Components;

namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public interface IFsOpenOperation
{
    /// <summary>
    /// Filter which determines if a file can be opened by this operation. True if it is compatible. Otherwise false
    /// </summary>
    public Func<FsEntry, bool> Filter { get; }
    
    /// <summary>
    /// Order index to overrule other potential compatibly operations. Higher value overrules lower value
    /// </summary>
    public int Order { get; }
    
    /// <summary>
    /// Checks if the current <see cref="IFsAccess"/> and <see cref="IFileManager"/> is compatible with this operation.
    /// If it returns false the file manager will ignore this operation completely.
    /// This should prevent invalid operations added in the <see cref="FileManager.OnConfigure"/> callback
    /// </summary>
    /// <param name="access"><see cref="IFsAccess"/> used by the file manager</param>
    /// <param name="fileManager">Reference to the file manager itself</param>
    /// <returns></returns>
    public bool CheckCompatability(IFsAccess access, IFileManager fileManager);
    
    /// <summary>
    /// Opens the specified file located in the specified working directory
    /// </summary>
    /// <param name="workingDir">Working directory the <see cref="FsEntry"/> is located in</param>
    /// <param name="entry">File which should be opened</param>
    /// <param name="fsAccess">Currently used <see cref="IFsAccess"/></param>
    /// <param name="fileManager">Reference to the file manager itself</param>
    /// <returns><b>null</b> if the file could not be opened otherwise it should returned the rendered open screen</returns>
    public Task<RenderFragment?> OpenAsync(string workingDir, FsEntry entry, IFsAccess fsAccess, IFileManager fileManager);
}