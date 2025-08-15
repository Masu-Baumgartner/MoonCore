namespace MoonCore.Blazor.FlyonUi.Test.Shared.Http.Responses;

public class FsEntryResponse
{
    public required string Name { get; set; }
    public required long Size { get; set; }
    public required bool IsFolder { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}