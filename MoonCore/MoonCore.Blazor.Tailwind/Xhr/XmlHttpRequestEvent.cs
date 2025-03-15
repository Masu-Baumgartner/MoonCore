using System.Text.Json.Serialization;

namespace MoonCore.Blazor.Tailwind.Xhr;

public class XmlHttpRequestEvent
{
    [JsonPropertyName("total")]
    public long Total { get; set; }
    
    [JsonPropertyName("loaded")]
    public long Loaded { get; set; }
}