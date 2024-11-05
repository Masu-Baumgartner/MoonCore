using System.Text.Json;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.Models;

namespace MoonCore.Extended.OAuth2.Provider;

public class OAuth2ProviderService
{
    private readonly OAuth2ProviderConfiguration ProviderConfiguration;

    public OAuth2ProviderService(OAuth2ProviderConfiguration providerConfiguration)
    {
        ProviderConfiguration = providerConfiguration;
    }

    public Task<string> GenerateCode(Action<Dictionary<string, object>> onConfigureData)
    {
        var jwt = JwtHelper.Encode(
            ProviderConfiguration.CodeSecret,
            onConfigureData,
            TimeSpan.FromMinutes(1)
        );

        return Task.FromResult(jwt);
    }

    public Task<bool> IsValidAuthorization(string clientId, string redirectUri)
    {
        if (clientId != ProviderConfiguration.ClientId)
            return Task.FromResult(false);

        if (redirectUri != ProviderConfiguration.AuthorizationRedirect)
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    public Task<AccessData?> ValidateAccess(
        string clientId,
        string clientSecret,
        string redirectUri,
        string code,
        Func<Dictionary<string, JsonElement>, bool> validateData,
        Action<Dictionary<string, object>> onConfigureTokens
    )
    {
        if (clientId != ProviderConfiguration.ClientId)
            return Task.FromResult<AccessData?>(null);

        if (clientSecret != ProviderConfiguration.ClientSecret)
            return Task.FromResult<AccessData?>(null);

        if (redirectUri != ProviderConfiguration.AuthorizationRedirect)
            return Task.FromResult<AccessData?>(null);

        if (!JwtHelper.TryVerifyAndDecodePayload(ProviderConfiguration.CodeSecret, code!, out var data))
            return Task.FromResult<AccessData?>(null);

        if (!validateData.Invoke(data))
            return Task.FromResult<AccessData?>(null);

        var tokenPair = TokenHelper.GeneratePair(
            ProviderConfiguration.AccessSecret,
            ProviderConfiguration.RefreshSecret,
            onConfigureTokens,
            ProviderConfiguration.AccessTokenDuration,
            ProviderConfiguration.RefreshTokenDuration
        );

        return Task.FromResult<AccessData?>(new AccessData()
        {
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresIn = ProviderConfiguration.AccessTokenDuration,
            TokenType = "unset"
        });
    }

    public Task<bool> IsValidAccessToken(string accessToken, Func<Dictionary<string, JsonElement>, bool> validateData)
        => Task.FromResult(TokenHelper.IsValidAccessToken(accessToken, ProviderConfiguration.AccessSecret, validateData));

    public Task<RefreshData?> RefreshAccess(
        string refreshToken,
        Func<Dictionary<string, JsonElement>, Dictionary<string, object>, bool> processData)
    {
        var pair = TokenHelper.RefreshPair(
            refreshToken,
            ProviderConfiguration.AccessSecret,
            ProviderConfiguration.RefreshSecret,
            processData,
            ProviderConfiguration.AccessTokenDuration,
            ProviderConfiguration.RefreshTokenDuration
        );

        if (!pair.HasValue)
            return Task.FromResult<RefreshData?>(null);

        return Task.FromResult<RefreshData?>(new RefreshData()
        {
            AccessToken = pair.Value.AccessToken,
            RefreshToken = pair.Value.RefreshToken,
            ExpiresIn = ProviderConfiguration.AccessTokenDuration,
            TokenType = "unset"
        });
    }
}