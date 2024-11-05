using MoonCore.Extended.OAuth2.Models;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.Consumer;

public class OAuth2ConsumerService
{
    private readonly OAuth2ConsumerConfiguration ConsumerConfiguration;
    private readonly HttpClient Client;

    public OAuth2ConsumerService(OAuth2ConsumerConfiguration consumerConfiguration, HttpClient client)
    {
        ConsumerConfiguration = consumerConfiguration;
        Client = client;
    }

    public Task<AuthorizationData> StartAuthorizing()
    {
        var result = new AuthorizationData()
        {
            Endpoint = ConsumerConfiguration.AuthorizationEndpoint,
            ClientId = ConsumerConfiguration.ClientId,
            RedirectUri = ConsumerConfiguration.AuthorizationRedirect
        };

        return Task.FromResult(result);
    }

    public async Task<AccessData> RequestAccess(string code)
    {
        var response = await Client.PostAsync(ConsumerConfiguration.AccessEndpoint, new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", ConsumerConfiguration.ClientId),
            new KeyValuePair<string, string>("client_secret", ConsumerConfiguration.ClientSecret),
            new KeyValuePair<string, string>("redirect_uri", ConsumerConfiguration.AuthorizationRedirect),
        }));
        
        await response.HandlePossibleApiError();

        return await response.ParseAsJson<AccessData>();
    }

    public async Task<RefreshData> RefreshAccess(string refreshToken)
    {
        var response = await Client.PostAsync(ConsumerConfiguration.RefreshEndpoint, new FormUrlEncodedContent(new []
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken)
        }));
        
        await response.HandlePossibleApiError();

        return await response.ParseAsJson<RefreshData>();
    }
}