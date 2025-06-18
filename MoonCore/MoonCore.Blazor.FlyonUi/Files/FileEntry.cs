namespace MoonCore.Blazor.FlyonUi.Files;

public class FileEntry
{
    public string Name { get; set; }
    public long Size { get; set; }
    public bool IsFolder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}