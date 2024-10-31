using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.Models;

namespace MoonCore.Extended.OAuth2.AuthServer;

public class OAuth2Service
{
    private readonly OAuth2Configuration Configuration;
    private readonly TokenHelper TokenHelper;

    public OAuth2Service(OAuth2Configuration configuration, TokenHelper tokenHelper)
    {
        Configuration = configuration;
        TokenHelper = tokenHelper;
    }

    public Task<string> GenerateCode(Action<Dictionary<string, object>> onConfigureData)
    {
        var jwt = JwtHelper.Encode(
            Configuration.CodeSecret,
            onConfigureData,
            TimeSpan.FromMinutes(1)
        );

        return Task.FromResult(jwt);
    }

    public Task<bool> IsValidAuthorization(string clientId, string redirectUri)
    {
        if (clientId != Configuration.ClientId)
            return Task.FromResult(false);

        if (redirectUri != Configuration.AuthorizationRedirect)
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
        if (clientId != Configuration.ClientId)
            return Task.FromResult<AccessData?>(null);

        if (clientSecret != Configuration.ClientSecret)
            return Task.FromResult<AccessData?>(null);

        if (redirectUri != Configuration.AuthorizationRedirect)
            return Task.FromResult<AccessData?>(null);

        if (!JwtHelper.TryVerifyAndDecodePayload(Configuration.CodeSecret, code!, out var data))
            return Task.FromResult<AccessData?>(null);

        if (!validateData.Invoke(data))
            return Task.FromResult<AccessData?>(null);

        var tokenPair = TokenHelper.GeneratePair(
            Configuration.AccessSecret,
            Configuration.RefreshSecret,
            onConfigureTokens,
            Configuration.AccessTokenDuration,
            Configuration.RefreshTokenDuration
        );

        return Task.FromResult<AccessData?>(new AccessData()
        {
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresIn = Configuration.AccessTokenDuration,
            TokenType = "unset"
        });
    }

    public Task<bool> IsValidAccessToken(string accessToken, Func<Dictionary<string, JsonElement>, bool> validateData)
        => Task.FromResult(TokenHelper.IsValidAccessToken(accessToken, Configuration.AccessSecret, validateData));

    public Task<RefreshData?> RefreshAccess(
        string refreshToken,
        Func<Dictionary<string, JsonElement>, Dictionary<string, object>, bool> processData)
    {
        var pair = TokenHelper.RefreshPair(
            refreshToken,
            Configuration.AccessSecret,
            Configuration.RefreshSecret,
            processData,
            Configuration.AccessTokenDuration,
            Configuration.RefreshTokenDuration
        );

        if (!pair.HasValue)
            return Task.FromResult<RefreshData?>(null);

        return Task.FromResult<RefreshData?>(new RefreshData()
        {
            AccessToken = pair.Value.AccessToken,
            RefreshToken = pair.Value.RefreshToken,
            ExpiresIn = Configuration.AccessTokenDuration,
            TokenType = "unset"
        });
    }
}