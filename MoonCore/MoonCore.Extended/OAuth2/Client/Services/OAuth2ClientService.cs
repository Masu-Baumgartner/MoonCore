using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using MoonCore.Extended.OAuth2.Client.Configuration;
using MoonCore.Extended.OAuth2.Client.Extensions;
using MoonCore.Extended.OAuth2.Client.Http.Response;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.Client.Services;

public class OAuth2ClientService
{
    private readonly OAuth2ClientConfiguration Configuration;

    public OAuth2ClientService(OAuth2ClientConfiguration configuration)
    {
        Configuration = configuration;
    }

    public async Task Login(HttpContext httpContext)
    {
        await Logout(httpContext);

        var redirectUrl = OAuth2Helper.GenerateRedirectUri(Configuration);
        
        httpContext.Response.Redirect(redirectUrl);
    }

    public async Task RequestAccess(HttpContext httpContext, string code)
    {
        var accessTokenUrl = OAuth2Helper.GenerateAccessTokenUri(Configuration, code);

        await CallAccessUri(httpContext, accessTokenUrl);
    }

    public async Task RefreshAccess(HttpContext httpContext)
    {
        var refreshToken = httpContext.Request.Cookies[Configuration.CookieNames.RefreshToken] ?? "unset";
        var uri = OAuth2Helper.GenerateRefreshTokenUri(Configuration, refreshToken);
        
        await CallAccessUri(httpContext, uri);
    }
    
    private async Task CallAccessUri(HttpContext httpContext, string uri)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(uri, null);

        var accessTokenResponse = (await response.Content.ReadFromJsonAsync<AccessTokenResponse>())!;
        
        await response.HandlePossibleApiError();
        
        httpContext.Response.Cookies.Append(Configuration.CookieNames.AccessToken, accessTokenResponse.AccessToken);
        httpContext.Response.Cookies.Append(Configuration.CookieNames.RefreshToken, accessTokenResponse.RefreshToken);
        httpContext.Response.Cookies.Append(Configuration.CookieNames.Expire, accessTokenResponse.ExpiresIn.ToString());
    }

    public Task Logout(HttpContext httpContext)
    {
        if(httpContext.Request.Cookies.ContainsKey(Configuration.CookieNames.AccessToken))
            httpContext.Response.Cookies.Delete(Configuration.CookieNames.AccessToken);
        
        if(httpContext.Request.Cookies.ContainsKey(Configuration.CookieNames.RefreshToken))
            httpContext.Response.Cookies.Delete(Configuration.CookieNames.RefreshToken);
        
        return Task.CompletedTask;
    }
}