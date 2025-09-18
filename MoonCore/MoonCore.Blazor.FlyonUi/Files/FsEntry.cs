namespace MoonCore.Blazor.FlyonUi.Files;

public record FsEntry
{
    /// <summary>
    /// Name of the entry
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Size of the entry in bytes
    /// </summary>
    public required long Size { get; set; }
    
    /// <summary>
    /// Whether an entry is a folder
    /// </summary>
    public required bool IsFolder { get; set; }
    
    /// <summary>
    /// Creation timestamp of an item
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Update timestamp of an item
    /// </summary>
    public required DateTimeOffset UpdatedAt { get; set; }
}