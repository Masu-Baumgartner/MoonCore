using Microsoft.Extensions.DependencyInjection;
using MoonCore.Exceptions;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.LocalProvider.Models;
using MoonCore.Extensions;
using MoonCore.Models;

namespace MoonCore.Extended.OAuth2.LocalProvider.Implementations;

public class LocalOAuth2Provider<T> : IOAuth2Provider<T> where T : IUserModel
{
    private readonly LocalProviderConfiguration Configuration;
    private readonly IServiceProvider ServiceProvider;
    private readonly IDataProvider<T> DataProvider;

    public LocalOAuth2Provider(
        IServiceProvider serviceProvider,
        LocalProviderConfiguration configuration,
        IDataProvider<T> dataProvider)
    {
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        DataProvider = dataProvider;
    }

    public async Task<TokenPair> ProcessAccess(string code)
    {
        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{Configuration.PublicUrl}/api/_auth/oauth2/access");

        request.Headers.Add("Authorization", Configuration.ClientSecret);

        request.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("redirect_uri", Configuration.RedirectUri),
            new KeyValuePair<string, string>("client_id", Configuration.ClientId),
        ]);

        var response = await httpClient.SendAsync(request);

        await response.HandlePossibleApiError();

        return await response.ParseAsJson<TokenPair>();
    }

    public async Task<TokenPair> ProcessRefresh(string refreshToken)
    {
        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{Configuration.PublicUrl}/api/_auth/oauth2/refresh");

        request.Headers.Add("Authorization", Configuration.ClientSecret);

        request.Content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("grant_type", "refresh_token")
        ]);

        var response = await httpClient.SendAsync(request);

        await response.HandlePossibleApiError();

        return await response.ParseAsJson<TokenPair>();
    }

    public async Task<T> ProcessSync(string accessToken)
    {
        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"{Configuration.PublicUrl}/api/_auth/oauth2/info");

        request.Headers.Add("Authorization", accessToken);

        var response = await httpClient.SendAsync(request);
        await response.HandlePossibleApiError();

        var infoResponse = await response.ParseAsJson<InfoResponse>();

        using var scope = ServiceProvider.CreateScope();
        var user = await DataProvider.LoadById(infoResponse.Id);

        if (user == null)
        {
            throw new HttpApiException(
                "Unable to find user in data source. As this is a local oauth2 implementation we cannot sync the user",
                500
            );
        }

        return user;
    }
}