using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Fm.Models;

public class TransferData
{
    [JsonPropertyName("path")] public string Path { get; set; }
    [JsonPropertyName("stream")] public IJSStreamReference? Stream { get; set; }
    [JsonPropertyName("left")] public int Left { get; set; }
}