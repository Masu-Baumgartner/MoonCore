using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Exceptions;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.Helpers.OAuth2.LocalPages;
using MoonCore.Extended.OAuth2.Models;
using MoonCore.Extended.OAuth2.Provider;
using MoonCore.Helpers;
using MoonCore.Http.Responses.OAuth2;

namespace MoonCore.Extended.Helpers.OAuth2;

public class LocalOAuth2ProviderControllerHelper
{
    public static async Task Authorize(
        HttpContext context,
        OAuth2ProviderService providerService,
        LocalOAuth2ProviderConfig config,
        string responseType,
        string clientId,
        string redirectUri,
        string action
        )
    {
        if (responseType != "code")
            throw new HttpApiException("Invalid response type", 400);

        if (!await providerService.IsValidAuthorization(clientId, redirectUri))
            throw new HttpApiException("Invalid authorization request", 400);
        
        if (!config.AllowRegister && action == "register")
            throw new HttpApiException("The register action is not allowed", 400);

        context.Response.StatusCode = 200;

        if (action == "register")
        {
            await context.Response.WriteAsync(
                await ComponentHelper.RenderComponent<Register>(context.RequestServices, parameters =>
                {
                    parameters.Add("ClientId", clientId);
                    parameters.Add("ResponseType", responseType);
                    parameters.Add("RedirectUri", redirectUri);
                })
            );
        }
        else
        {
            await context.Response.WriteAsync(
                await ComponentHelper.RenderComponent<Login>(context.RequestServices, parameters =>
                {
                    parameters.Add("ClientId", clientId);
                    parameters.Add("ResponseType", responseType);
                    parameters.Add("RedirectUri", redirectUri);
                    parameters.Add("ShowRegister", config.AllowRegister);
                })
            );
        }
    }

    public static async Task AuthorizePost(
        HttpContext context,
        OAuth2ProviderService providerService,
        LocalOAuth2ProviderConfig config,
        string email,
        string username,
        string password,
        string responseType,
        string clientId,
        string redirectUri,
        string action
    )
    {
        if (responseType != "code")
            throw new HttpApiException("Invalid response type", 400);

        if (!await providerService.IsValidAuthorization(clientId, redirectUri))
            throw new HttpApiException("Invalid authorization request", 400);

        if (!config.AllowRegister && action == "register")
            throw new HttpApiException("The register action is not allowed", 400);

        int userId;

        try
        {
            if (action == "register")
                userId = await config.HandleRegister.Invoke(username, email, password);
            else
                userId = await config.HandleLogin.Invoke(email, password);
        }
        catch (HttpApiException e)
        {
            if (action == "register")
            {
                await context.Response.WriteAsync(
                    await ComponentHelper.RenderComponent<Register>(context.RequestServices, parameters =>
                    {
                        parameters.Add("ClientId", clientId);
                        parameters.Add("ResponseType", responseType);
                        parameters.Add("RedirectUri", redirectUri);
                        parameters.Add("Error", e.Title ?? "An unknown error occured");
                    })
                );
            }
            else
            {
                await context.Response.WriteAsync(
                    await ComponentHelper.RenderComponent<Login>(context.RequestServices, parameters =>
                    {
                        parameters.Add("ClientId", clientId);
                        parameters.Add("ResponseType", responseType);
                        parameters.Add("RedirectUri", redirectUri);
                        parameters.Add("ShowRegister", config.AllowRegister);
                        parameters.Add("Error", e.Title ?? "An unknown error occured");
                    })
                );
            }
            
            return;
        }

        var code = await providerService.GenerateCode(data => { data.Add("userId", userId); });

        var redirectUrl = redirectUri +
                          $"?code={code}";

        context.Response.Redirect(redirectUrl);
    }

    public static async Task<AccessData> Access(
        OAuth2ProviderService providerService,
        LocalOAuth2ProviderConfig config,
        [FromForm(Name = "client_id")] string clientId,
        [FromForm(Name = "client_secret")] string clientSecret,
        [FromForm(Name = "redirect_uri")] string redirectUri,
        [FromForm(Name = "grant_type")] string grantType,
        [FromForm(Name = "code")] string code
    )
    {
        if (grantType != "authorization_code")
            throw new HttpApiException("Invalid grant type", 400);

        var userId = -1;
        
        var access = await providerService.ValidateAccess(clientId, clientSecret, redirectUri, code, data =>
        {
            if (!data.TryGetValue("userId", out var userIdStr) || !userIdStr.TryGetInt32(out var id))
                return false;

            userId = id;
            
            return config.Validate.Invoke(id).Result;
        }, data => { data.Add("userId", userId); });

        if (access == null)
            throw new HttpApiException("Unable to validate access", 400);

        return access;
    }

    public static async Task<RefreshData> Refresh(
        OAuth2ProviderService providerService,
        LocalOAuth2ProviderConfig config,
        [FromForm(Name = "grant_type")] string grantType,
        [FromForm(Name = "refresh_token")] string refreshToken
        )
    {
        if (grantType != "refresh_token")
            throw new HttpApiException("Invalid grant type", 400);

        var refreshData = await providerService.RefreshAccess(refreshToken, (refreshTokenData, newTokenData) =>
        {
            // Check if the userId is present in the refresh token
            if (!refreshTokenData.TryGetValue("userId", out var userIdStr) || !userIdStr.TryGetInt32(out var userId))
                return false;

            if (!config.Validate.Invoke(userId).Result)
                return false;
            
            newTokenData.Add("userId", userId);
            return true;
        });
        
        if(refreshData == null)
            throw new HttpApiException("Unable to validate refresh", 400);

        return refreshData;
    }

    public static async Task<LocalOAuth2InfoResponse> Info(
        HttpContext context,
        OAuth2ProviderService providerService,
        LocalOAuth2ProviderConfig config
        )
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
            throw new HttpApiException("Authorization header is missing", 400);

        var authHeader = context.Request.Headers["Authorization"].First() ?? "";

        if (string.IsNullOrEmpty(authHeader))
            throw new HttpApiException("Authorization header is missing", 400);

        var userId = -1;

        var isValid = await providerService.IsValidAccessToken(
            authHeader,
            data =>
            {
                // Check if the userId is present in the access token
                if (!data.TryGetValue("userId", out var userIdStr) || !userIdStr.TryGetInt32(out var id))
                    return false;

                userId = id;

                return config.Validate.Invoke(userId).Result;
            }
        );

        if (!isValid)
            throw new HttpApiException("Invalid access token", 401);

        var currentUser = await config.LoadUserData.Invoke(userId);

        return new()
        {
            Username = currentUser.Username,
            Email = currentUser.Email
        };
    }
}