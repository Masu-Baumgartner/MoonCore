using System.Text.Json.Serialization;

namespace MoonCore.Extended.OAuth2.ApiServer.Models;

public class AccessData
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    [JsonPropertyName("token_type")] public string TokenType { get; set; } = "unset";
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
}