namespace MoonCore.Blazor.FlyonUi.Files.Manager.Abstractions;

public record ArchiveFormat
{
    public string Identifier { get; set; }
    public string[] Extensions { get; set; }
    public string DisplayName { get; set; }
}