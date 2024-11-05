namespace MoonCore.Extended.OAuth2.Provider;

public class OAuth2ProviderConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AuthorizationRedirect { get; set; }
    public string CodeSecret { get; set; }
    public string AccessSecret { get; set; }
    public string RefreshSecret { get; set; }
    public int AccessTokenDuration { get; set; }
    public int RefreshTokenDuration { get; set; }
}