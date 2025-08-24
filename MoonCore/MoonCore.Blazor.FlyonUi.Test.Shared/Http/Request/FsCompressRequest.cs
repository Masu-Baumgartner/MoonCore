namespace MoonCore.Blazor.FlyonUi.Test.Shared.Http.Request;

public class FsCompressRequest
{
    public string Destination { get; set; }
    public string Root { get; set; }
    public string[] Files { get; set; }
    public string Identifier { get; set; }
}