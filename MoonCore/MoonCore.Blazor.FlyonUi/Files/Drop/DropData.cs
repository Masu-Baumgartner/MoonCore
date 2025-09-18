using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.FlyonUi.Files.Drop;

public record DropData
{
    /// <summary>
    /// Relative path provided by the browser
    /// </summary>
    [JsonPropertyName("path")] public string Path { get; set; }
    
    /// <summary>
    /// Stream reference to open the file stream and retrieve the data
    /// </summary>
    [JsonPropertyName("stream")] public IJSStreamReference Stream { get; set; }
    
    /// <summary>
    /// Allows the interop to specify if it should skip to the next item (e.g. in case of an invalid item)
    /// </summary>
    [JsonPropertyName("shouldSkipToNext")] public bool ShouldSkipToNext { get; set; } = false;
}