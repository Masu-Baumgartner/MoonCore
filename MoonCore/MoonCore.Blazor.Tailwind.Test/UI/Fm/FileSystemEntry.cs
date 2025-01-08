namespace MoonCore.Blazor.Tailwind.Test.UI.Fm;

public class FileSystemEntry
{
    public string Name { get; set; }
    public bool IsFile { get; set; }
    public long Size { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}