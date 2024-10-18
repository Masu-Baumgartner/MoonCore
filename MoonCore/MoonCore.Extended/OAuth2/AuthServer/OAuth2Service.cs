using Microsoft.AspNetCore.Http;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.Models;

namespace MoonCore.Extended.OAuth2.AuthServer;

public class OAuth2Service
{
    private readonly OAuth2Configuration Configuration;
    private readonly TokenHelper TokenHelper;
    private readonly JwtHelper JwtHelper;

    public OAuth2Service(OAuth2Configuration configuration, JwtHelper jwtHelper, TokenHelper tokenHelper)
    {
        Configuration = configuration;
        JwtHelper = jwtHelper;
        TokenHelper = tokenHelper;
    }

    public async Task<string> GenerateCode(Action<Dictionary<string, string>> onConfigureData)
    {
        var jwt = await JwtHelper.Create(
            Configuration.CodeSecret,
            onConfigureData,
            TimeSpan.FromMinutes(1)
        );

        return jwt;
    }

    public Task<bool> IsValidAuthorization(string clientId, string redirectUri)
    {
        if (clientId != Configuration.ClientId)
            return Task.FromResult(false);
        
        if (redirectUri != Configuration.AuthorizationRedirect)
            return Task.FromResult(false);
        
        return Task.FromResult(true);
    }

    public async Task<AccessData?> ValidateAccess(string clientId, string clientSecret, string redirectUri, string code,
        Func<Dictionary<string, string>, bool> validateData, Action<Dictionary<string, string>> onConfigureTokens)
    {
        if (clientId != Configuration.ClientId)
            return null;

        if (clientSecret != Configuration.ClientSecret)
            return null;

        if (redirectUri != Configuration.AuthorizationRedirect)
            return null;

        if (!await JwtHelper.Validate(Configuration.CodeSecret, code!))
            return null;

        var data = await JwtHelper.Decode(code!);

        if (!validateData.Invoke(data))
            return null;

        var tokenPair = await TokenHelper.GeneratePair(
            Configuration.AccessSecret,
            Configuration.RefreshSecret,
            onConfigureTokens,
            Configuration.AccessTokenDuration,
            Configuration.RefreshTokenDuration
        );

        return new()
        {
            AccessToken = tokenPair.AccessToken,
            RefreshToken = tokenPair.RefreshToken,
            ExpiresIn = Configuration.AccessTokenDuration,
            TokenType = "unset"
        };
    }

    public Task<bool> IsValidAccessToken(string accessToken, Func<Dictionary<string, string>, bool> validateData)
        => TokenHelper.IsValidAccessToken(accessToken, Configuration.AccessSecret, validateData);

    public async Task<RefreshData?> RefreshAccess(
        string refreshToken,
        Func<Dictionary<string, string>, Dictionary<string, string>, bool> onConfigureToken)
    {
        var pair = await TokenHelper.RefreshPair(
            refreshToken,
            Configuration.AccessSecret,
            Configuration.RefreshSecret,
            onConfigureToken,
            Configuration.AccessTokenDuration,
            Configuration.RefreshTokenDuration
        );

        if (!pair.HasValue)
            return null;

        return new()
        {
            AccessToken = pair.Value.AccessToken,
            RefreshToken = pair.Value.RefreshToken,
            ExpiresIn = Configuration.AccessTokenDuration,
            TokenType = "unset"
        };
    }
}