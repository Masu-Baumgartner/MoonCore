using MoonCore.Extended.OAuth2.Models;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.ApiServer;

public class OAuth2Service
{
    private readonly OAuth2Configuration Configuration;
    private readonly HttpClient Client;

    public OAuth2Service(OAuth2Configuration configuration, HttpClient client)
    {
        Configuration = configuration;
        Client = client;
    }

    public Task<AuthorizationData> StartAuthorizing()
    {
        var result = new AuthorizationData()
        {
            Endpoint = Configuration.AuthorizationEndpoint,
            ClientId = Configuration.ClientId,
            RedirectUri = Configuration.AuthorizationRedirect
        };

        return Task.FromResult(result);
    }

    public async Task<AccessData> RequestAccess(string code)
    {
        var response = await Client.PostAsync(Configuration.AccessEndpoint, new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", Configuration.ClientId),
            new KeyValuePair<string, string>("client_secret", Configuration.ClientSecret),
            new KeyValuePair<string, string>("redirect_uri", Configuration.AuthorizationRedirect),
        }));
        
        await response.HandlePossibleApiError();

        return await response.ParseAsJson<AccessData>();
    }

    public async Task<RefreshData> RefreshAccess(string refreshToken)
    {
        var response = await Client.PostAsync(Configuration.RefreshEndpoint, new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        }));
        
        await response.HandlePossibleApiError();

        return await response.ParseAsJson<RefreshData>();
    }
}