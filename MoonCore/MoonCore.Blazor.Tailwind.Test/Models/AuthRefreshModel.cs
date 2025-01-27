using System.Text.Json.Serialization;

namespace MoonCore.Blazor.Tailwind.Test.Models;

public class AuthRefreshModel
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}