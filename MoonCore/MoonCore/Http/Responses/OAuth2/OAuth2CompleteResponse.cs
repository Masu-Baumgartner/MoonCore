namespace MoonCore.Http.Responses.OAuth2;

public class OAuth2CompleteResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}