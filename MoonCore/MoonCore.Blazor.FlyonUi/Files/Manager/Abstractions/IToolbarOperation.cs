namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Defines the functionality of a toolbar operation which can be run at any time and have only the current
/// working directory as an execution parameter
/// </summary>
public interface IToolbarOperation : IFsOperation
{
    /// <summary>
    /// Css classes to use for the toolbar button
    /// </summary>
    public string ToolbarCss { get; }

    /// <summary>
    /// Executes the operation in the specified working directory
    /// </summary>
    /// <param name="workingDir">Working directory the operation should run in</param>
    /// <param name="access">Currently used <see cref="IFsAccess"/></param>
    /// <param name="fileManager">Reference to the file manager itself</param>
    /// <returns></returns>
    public Task ExecuteAsync(string workingDir, IFsAccess access, IFileManager fileManager);
}