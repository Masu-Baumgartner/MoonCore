namespace MoonCore.Models;

public class HttpApiErrorModel
{
    public string Title { get; set; } = "";
    public int Status { get; set; }
    public string Detail { get; set; } = "";
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}