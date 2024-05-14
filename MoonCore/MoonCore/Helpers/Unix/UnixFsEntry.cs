namespace MoonCore.Helpers.Unix;

public class UnixFsEntry
{
    public string Name { get; set; }
    public long Size { get; set; }
    public bool IsFile { get; set; }
    public bool IsDirectory { get; set; }
    public DateTime LastChanged { get; set; }
    public DateTime CreatedAt { get; set; }
}