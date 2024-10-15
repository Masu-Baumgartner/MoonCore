using Microsoft.AspNetCore.Http;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.AuthServer.Models;

namespace MoonCore.Extended.OAuth2.AuthServer;

public class OAuth2Service
{
    private readonly OAuth2Configuration Configuration;
    private readonly JwtHelper JwtHelper;

    public OAuth2Service(OAuth2Configuration configuration, JwtHelper jwtHelper)
    {
        Configuration = configuration;
        JwtHelper = jwtHelper;
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

    public async Task<AccessData?> ValidateAccess(HttpRequest request, Func<Dictionary<string, string>, bool> validateData, Action<Dictionary<string, string>> onConfigureAccessToken, Action<Dictionary<string, string>> onConfigureRefreshAccess)
    {
        if (!request.Form.TryGetValue("client_id", out var clientId) || clientId != Configuration.ClientId)
            return null;
        
        if (!request.Form.TryGetValue("client_secret", out var clientSecret) || clientSecret != Configuration.ClientSecret)
            return null;
        
        if (!request.Form.TryGetValue("redirect_uri", out var redirectUri) || redirectUri != Configuration.AuthorizationRedirect)
            return null;
        
        if (!request.Form.TryGetValue("code", out var code))
            return null;

        if (!await JwtHelper.Validate(Configuration.CodeSecret, code!))
            return null;

        var data = await JwtHelper.Decode(code!);

        if (!validateData.Invoke(data))
            return null;
        
        
    }
}