namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

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

    public string Identifier { get; set; }
    public string[] Extensions { get; set; }
    public string DisplayName { get; set; }
}