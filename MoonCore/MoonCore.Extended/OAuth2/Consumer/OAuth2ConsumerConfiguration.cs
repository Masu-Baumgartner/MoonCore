using MoonCore.Extended.OAuth2.Models;

namespace MoonCore.Extended.OAuth2.Consumer;

public class OAuth2ConsumerConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    public string AuthorizationEndpoint { get; set; }
    public string AuthorizationRedirect { get; set; }
    public string AccessEndpoint { get; set; }
    public string RefreshEndpoint { get; set; }
    
    public Func<IServiceProvider, AccessData, Task<Dictionary<string, object>>> ProcessComplete { get; set; }
}