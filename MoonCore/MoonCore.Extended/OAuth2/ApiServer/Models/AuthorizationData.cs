namespace MoonCore.Extended.OAuth2.ApiServer.Models;

public class AuthorizationData
{
    public string ClientId { get; set; }
    public string Endpoint { get; set; }
    public string RedirectUri { get; set; }
}