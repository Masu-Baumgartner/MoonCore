namespace MoonCore.Extended.OAuth2.Client.Configuration;

public class OAuth2ClientConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RedirectUri { get; set; }
    
    public CookieNamesData CookieNames { get; set; } = new();
    public EndpointsData Endpoints { get; set; } = new();
    
    public class CookieNamesData
    {
        public string AccessToken { get; set; } = "access-token";
        public string RefreshToken { get; set; } = "refresh-token";
        public string Expire { get; set; } = "token-expire";
    }
    
    public class EndpointsData
    {
        public string AuthorizationUri { get; set; }
        public string AccessTokenUri { get; set; }
        public string RefreshTokenUri { get; set; }
    }
}