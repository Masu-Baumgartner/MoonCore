using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace MoonCore.Blazor.Tailwind.Fm.Models;

public struct TransferData
{
    [JsonPropertyName("path")] public string Path { get; set; }
    [JsonPropertyName("streamRef")] public IJSStreamReference? StreamRef { get; set; }
}