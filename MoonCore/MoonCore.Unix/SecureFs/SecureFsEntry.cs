namespace MoonCore.Unix.SecureFs;

public class SecureFsEntry
{
    public string Name { get; set; }
    public long Size { get; set; }
    public bool IsFile { get; set; }
    public bool IsDirectory { get; set; }
    public DateTime LastChanged { get; set; }
    public DateTime CreatedAt { get; set; }
    public int OwnerUserId { get; set; }
    public int OwnerGroupId { get; set; }
}