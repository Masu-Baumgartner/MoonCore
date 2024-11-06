namespace MoonCore.Http.Responses.TokenAuthentication;

public class RefreshResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}