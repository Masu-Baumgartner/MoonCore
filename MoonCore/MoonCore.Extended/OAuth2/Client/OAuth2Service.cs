using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.Client.Configuration;
using MoonCore.Extended.OAuth2.Client.Data;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.Client;

public class OAuth2Service
{
    private readonly OAuth2Configuration Configuration;
    private readonly JwtHelper JwtHelper;

    public OAuth2Service(OAuth2Configuration configuration, JwtHelper jwtHelper)
    {
        Configuration = configuration;
        JwtHelper = jwtHelper;
    }

    public Task<string> GetLoginUrl()
    {
        var uri = Configuration.Endpoints.Authorisation
               + "?response_type=code" +
               $"&client_id={Configuration.ClientId}" +
               $"redirect_uri={Configuration.Endpoints.Redirect}";

        return Task.FromResult(uri);
    }

    public async Task<bool> VerifyAccess(HttpContext context)
    {
        var accessToken = context.Request.Cookies[Configuration.CookieNames.Access] ?? "unset";

        if (Configuration.VerifyAccessToken != null)
            return await Configuration.VerifyAccessToken.Invoke(accessToken);
        else
            return await VerifyJwtAccessToken(accessToken);
    }

    public async Task RefreshAccess(HttpContext context)
    {
        var refreshToken = context.Request.Cookies[Configuration.CookieNames.Refresh] ?? "unset";
        
        var uri = Configuration.Endpoints.Refresh
                  + "?grant_type=refresh_token" +
                  $"&refresh_token={refreshToken}";

        var response = await MakeAccessRequest(uri);
        
        context.Response.Cookies.Append(Configuration.CookieNames.Refresh, response.RefreshToken);
        context.Response.Cookies.Append(Configuration.CookieNames.Access, response.AccessToken);
    }

    public async Task RequestAccess(HttpContext context, string code)
    {
        var uri = Configuration.Endpoints.Refresh
                  + "?grant_type=authorization_code" +
                  $"&code={code}" +
                  $"&client_id={Configuration.ClientId}" +
                  $"&redirect_uri={Configuration.Endpoints.Redirect}";

        var response = await MakeAccessRequest(uri);
        //TODO: Abstraction
        context.Response.Cookies.Append(Configuration.CookieNames.Refresh, response.RefreshToken);
        context.Response.Cookies.Append(Configuration.CookieNames.Access, response.AccessToken);
    }

    private async Task<AccessResponse> MakeAccessRequest(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, null);

        await response.HandlePossibleApiError();
        
        var responseText = await response.Content.ReadAsStringAsync();
        var accessResponse = JsonSerializer.Deserialize<AccessResponse>(responseText)!;

        return accessResponse;
    }
    
    // Helpers

    private async Task<bool> VerifyJwtAccessToken(string jwt)
        => await JwtHelper.Validate(Configuration.ClientSecret, jwt);
}