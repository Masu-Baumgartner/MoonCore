using System.Text.Json.Serialization;

namespace MoonCore.Blazor.Tailwind.Ace;

public class CodeEditorOptions
{
    [JsonPropertyName("newLineMode")]
    public string NewLineMode { get; set; } = "unix";

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("fontSize")]
    public int FontSize { get; set; } = 14;

    [JsonPropertyName("theme")]
    public string? Theme { get; set; }

    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }

    [JsonPropertyName("showPrintMargin")]
    public bool ShowPrintMargin { get; set; } = false;
}