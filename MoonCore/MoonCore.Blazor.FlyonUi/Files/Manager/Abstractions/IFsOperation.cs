namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Base definition of a file system operation
/// </summary>
public interface IFsOperation
{
    /// <summary>
    /// Name of the operation. Used for the UI of the file manager
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Icon to use in the UI. For a reference, look <see href="https://lucide.dev/icons">here</see>
    /// </summary>
    public string Icon { get; }
    
    /// <summary>
    /// Order index to overrule other potential compatibly operations. Higher value overrules lower value
    /// </summary>
    public int Order { get; }

    public bool CheckCompatability(IFsAccess access, IFileManager fileManager);
}