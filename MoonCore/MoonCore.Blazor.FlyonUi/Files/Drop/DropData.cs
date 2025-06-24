using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Files.Drop;

public record DropData
{
    [JsonPropertyName("path")] public string Path { get; set; }
    [JsonPropertyName("stream")] public IJSStreamReference Stream { get; set; }
    [JsonPropertyName("shouldSkipToNext")] public bool ShouldSkipToNext { get; set; } = false;
}