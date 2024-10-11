using System.Text.Json.Serialization;

namespace MoonCore.Extended.OAuth2.Client.Http.Response;

public class AccessTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "unset";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; } = 3600;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}