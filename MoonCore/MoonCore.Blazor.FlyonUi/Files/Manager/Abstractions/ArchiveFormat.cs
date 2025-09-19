namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

/// <summary>
/// Represents an archive format (like zip or tar) which the <see cref="IArchiveAccess"/> suports
/// </summary>
public record ArchiveFormat
{
    public ArchiveFormat(string identifier, string[] extensions, string displayName)
    {
        if (extensions.Length == 0)
            throw new ArgumentException("There needs to be at least one extension defined");
        
        Identifier = identifier;
        Extensions = extensions;
        DisplayName = displayName;
    }

    /// <summary>
    /// Internally used id to specify which archive format to use
    /// </summary>
    public string Identifier { get; set; }
    
    /// <summary>
    /// Array of file extensions the archive format supports. The file extensions <b>must not</b> start with a dot
    /// </summary>
    public string[] Extensions { get; set; }
    
    /// <summary>
    /// Display name of the format. This will be shown to the user
    /// </summary>
    public string DisplayName { get; set; }
}