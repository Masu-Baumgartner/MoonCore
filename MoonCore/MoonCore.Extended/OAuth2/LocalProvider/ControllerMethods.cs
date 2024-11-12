using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Exceptions;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.LocalProvider.Models;
using MoonCore.Models;

namespace MoonCore.Extended.OAuth2.LocalProvider;

public class ControllerMethods
{
    public static async Task Authorize<T>(
        HttpContext context,
        LocalProviderService<T> localProviderService,
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "redirect_uri")] string redirectUri,
        [FromQuery(Name = "response_type")] string responseType,
        [FromQuery(Name = "action")] string action = "login"
    ) where T : IUserModel
    {
        if (responseType != "code")
            throw new HttpApiException("Invalid response type", 400);

        await localProviderService.GetAuthorize(context, clientId, redirectUri, action);
    }

    public static async Task AuthorizePost<T>(
        HttpContext context,
        LocalProviderService<T> localProviderService,
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "redirect_uri")] string redirectUri,
        [FromQuery(Name = "response_type")] string responseType,
        [FromForm(Name = "email")] string email,
        [FromForm(Name = "password")] string password,
        [FromForm(Name = "username")] string? username = null,
        [FromQuery(Name = "action")] string action = "login"
    ) where T : IUserModel
    {
        if (responseType != "code")
            throw new HttpApiException("Invalid response type", 400);

        await localProviderService.HandleAuthorize(context, username, email, password, clientId, redirectUri, action);
    }

    public static async Task<TokenPair> HandleAccess<T>(
        HttpContext context,
        LocalProviderService<T> localProviderService,
        [FromForm(Name = "code")] string code
    ) where T : IUserModel
    {
        return await localProviderService.HandleAccess(context, code);
    }

    public static async Task<TokenPair> HandleRefresh<T>(
        HttpContext context,
        LocalProviderService<T> localProviderService,
        [FromForm(Name = "grant_type")] string grantType,
        [FromForm(Name = "refresh_token")] string refreshToken
    ) where T : IUserModel
    {
        if (grantType != "refresh_token")
            throw new HttpApiException("Invalid grant type", 400);

        return await localProviderService.HandleRefresh(context, refreshToken);
    }

    public static async Task<InfoResponse> HandleInfo<T>(
        HttpContext context,
        LocalProviderService<T> localProviderService
    ) where T : IUserModel
    {
        return await localProviderService.Info(context);
    }
}