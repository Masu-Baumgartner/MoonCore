namespace MoonCore.Extended.OAuth2.ApiServer;

public class OAuth2Configuration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public string AuthorizationEndpoint { get; set; }
    public string AuthorizationRedirect { get; set; }
    public string AccessEndpoint { get; set; }
    public string RefreshEndpoint { get; set; }
}