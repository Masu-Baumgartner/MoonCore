using MoonCore.Extended.Configuration;
using MoonCore.Extended.Interfaces;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Http.Requests.OAuth2;
using MoonCore.Http.Responses.OAuth2;

namespace MoonCore.Extended.Helpers;

public static class OAuth2ControllerHelper
{
    public static async Task<OAuth2StartResponse> Start(OAuth2ConsumerService oAuth2ConsumerService)
    {
        var data = await oAuth2ConsumerService.StartAuthorizing();

        return new OAuth2StartResponse()
        {
            ClientId = data.ClientId,
            Endpoint = data.Endpoint,
            RedirectUri = data.RedirectUri
        };
    }

    public static async Task<OAuth2CompleteResponse> Complete(
        OAuth2CompleteRequest request,
        OAuth2ConsumerService oAuth2ConsumerService,
        IServiceProvider serviceProvider,
        IOAuth2Provider provider,
        TokenAuthenticationConfig config
    )
    {
        var accessData = await oAuth2ConsumerService.RequestAccess(request.Code);
        var data = await provider.Sync(serviceProvider, accessData);
        
        // Generate local token-pair for the authentication
        // between client and the api server

        var tokenPair = TokenHelper.GeneratePair(
            config.AccessSecret,
            config.RefreshSecret,
            data,
            config.AccessDuration,
            config.RefreshDuration
        );

        // Authentication finished. Return data to client

        return new OAuth2CompleteResponse()
        {
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddSeconds(config.AccessDuration)
        };
    }
}