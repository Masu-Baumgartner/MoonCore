namespace MoonCore.Blazor.FlyonUi.Files;

public class FsEntry
{
    public required string Name { get; set; }
    public required long Size { get; set; }
    public required bool IsFolder { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}