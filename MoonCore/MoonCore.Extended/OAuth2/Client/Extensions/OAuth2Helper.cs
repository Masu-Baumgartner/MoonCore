using MoonCore.Extended.OAuth2.Client.Configuration;

namespace MoonCore.Extended.OAuth2.Client.Extensions;

public static class OAuth2Helper
{
    public static string GenerateRedirectUri(OAuth2ClientConfiguration configuration)
    {
        return configuration.Endpoints.AuthorizationUri +
               "?response_type=code" +
               $"&client_id={configuration.ClientId}" +
               $"&redirect_uri={configuration.RedirectUri}";
    }

    public static string GenerateAccessTokenUri(OAuth2ClientConfiguration configuration, string code)
    {
        return configuration.Endpoints.AccessTokenUri +
               "?grant_type=authorization_code" +
               $"&client_id={configuration.ClientId}" +
               $"&redirect_uri={configuration.RedirectUri}" +
               $"&code={code}";
    }

    public static string GenerateRefreshTokenUri(OAuth2ClientConfiguration configuration, string refreshToken)
    {
        return configuration.Endpoints.RefreshTokenUri +
               "?grant_type=refresh_token" +
               $"&refresh_token={refreshToken}";
    }
}