namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Use this interface in a file operation to invoke actions like <see cref="RefreshAsync"/> on the file manager
/// during the runtime of the operation
/// </summary>
public interface IFileManager
{
    /// <summary>
    /// Refreshes the file list by fetching it from the <see cref="IFsAccess"/>
    /// </summary>
    /// <returns></returns>
    public Task RefreshAsync();
    
    /// <summary>
    /// Closes the current screen opened by an <see cref="IFsOpenOperation"/>
    /// </summary>
    /// <returns></returns>
    public Task CloseOpenScreenAsync();
}