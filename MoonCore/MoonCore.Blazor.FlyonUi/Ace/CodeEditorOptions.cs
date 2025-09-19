using System.Text.Json.Serialization;

namespace MoonCore.Blazor.FlyonUi.Ace;

public class CodeEditorOptions
{
    /// <summary>
    /// Defines the new line separator
    /// More details can be found
    /// <see href="https://github.com/ajaxorg/ace/wiki/Configuring-Ace">here</see>
    /// </summary>
    [JsonPropertyName("newLineMode")]
    public string NewLineMode { get; set; } = "unix";

    /// <summary>
    /// Specifies the mode of the editor. This is used to provide correct syntax highlighting
    /// More details can be found
    /// <see href="https://github.com/ajaxorg/ace/wiki/Configuring-Ace">here</see>
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// Defines the font size to use
    /// More details can be found
    /// <see href="https://github.com/ajaxorg/ace/wiki/Configuring-Ace">here</see>
    /// </summary>
    [JsonPropertyName("fontSize")]
    public int FontSize { get; set; } = 14;

    /// <summary>
    /// Specifies the theme to use for ace. By default, the <b>ace/theme/mooncore</b> is used
    /// More details can be found
    /// <see href="https://github.com/ajaxorg/ace/wiki/Configuring-Ace">here</see>
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; set; }

    
    /// <summary>
    /// Defines the font family to use. Has to be already loaded via a css font import
    /// More details can be found
    /// <see href="https://github.com/ajaxorg/ace/wiki/Configuring-Ace">here</see>
    /// </summary>
    [JsonPropertyName("fontFamily")]
    public string? FontFamily { get; set; }

    
    /// <summary>
    /// Details can be found
    /// <see href="https://github.com/ajaxorg/ace/wiki/Configuring-Ace">here</see>
    /// </summary>
    [JsonPropertyName("showPrintMargin")]
    public bool ShowPrintMargin { get; set; } = false;
}