namespace MoonCore.Extended.OAuth2.Client.Configuration;

public class OAuth2Configuration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public EndpointsData Endpoints { get; set; } = new();
    public CookieNamesData CookieNames { get; set; } = new();

    public Func<string, Task<bool>>? VerifyAccessToken { get; set; }
    
    public class EndpointsData
    {
        public string Authorisation { get; set; }
        public string Refresh { get; set; }
        public string Access { get; set; }
        public string Redirect { get; set; }
    }
    
    public class CookieNamesData
    {
        public string Refresh { get; set; }
        public string Access { get; set; }
    }
}