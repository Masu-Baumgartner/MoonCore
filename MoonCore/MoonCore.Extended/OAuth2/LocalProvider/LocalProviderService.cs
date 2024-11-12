using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Exceptions;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.LocalProvider.Models;
using MoonCore.Extended.OAuth2.LocalProvider.Pages;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.Models;

namespace MoonCore.Extended.OAuth2.LocalProvider;

public class LocalProviderService<T> where T : IUserModel
{
    private readonly LocalProviderConfiguration Configuration;
    private readonly ILogger<LocalProviderService<T>> Logger;

    public LocalProviderService(
        LocalProviderConfiguration configuration,
        ILogger<LocalProviderService<T>> logger
    )
    {
        Configuration = configuration;
        Logger = logger;
    }

    public Task<bool> ValidateAuthorize(string clientId, string redirectUri)
    {
        if (clientId != Configuration.ClientId || redirectUri != Configuration.RedirectUri)
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    public async Task GetAuthorize(
        HttpContext context,
        string clientId,
        string redirectUri,
        string action = "login"
    )
    {
        if (!await ValidateAuthorize(clientId, redirectUri))
            throw new HttpApiException("Invalid authorization request", 400);

        if (!Configuration.AllowRegister && action == "register")
            throw new HttpApiException("The register action is not allowed", 400);

        context.Response.StatusCode = 200;

        if (action == "register")
        {
            await context.Response.WriteAsync(
                await ComponentHelper.RenderComponent<Register>(context.RequestServices, parameters =>
                {
                    parameters.Add("ClientId", clientId);
                    parameters.Add("ResponseType", "code");
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
                    parameters.Add("ResponseType", "code");
                    parameters.Add("RedirectUri", redirectUri);
                    parameters.Add("ShowRegister", Configuration.AllowRegister);
                })
            );
        }
    }

    public async Task HandleAuthorize(
        HttpContext context,
        string? username,
        string email,
        string password,
        string clientId,
        string redirectUri,
        string action)
    {
        if (!await ValidateAuthorize(clientId, redirectUri))
            throw new HttpApiException("Invalid authorization request", 400);

        if (!Configuration.AllowRegister && action == "register")
            throw new HttpApiException("The register action is not allowed", 400);

        if (action == "register" && string.IsNullOrEmpty(username))
            throw new HttpApiException("You need to provide a username", 400);

        var implementation = context.RequestServices.GetRequiredService<ILocalProviderImplementation<T>>();
        
        T model;

        try
        {
            if (action == "register")
                model = await implementation.Register(username, email, password);
            else
                model = await implementation.Login(email, password);
        }
        catch (HttpApiException e)
        {
            if (action == "register")
            {
                await context.Response.WriteAsync(
                    await ComponentHelper.RenderComponent<Register>(context.RequestServices, parameters =>
                    {
                        parameters.Add("ClientId", clientId);
                        parameters.Add("ResponseType", "code");
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
                        parameters.Add("ResponseType", "code");
                        parameters.Add("RedirectUri", redirectUri);
                        parameters.Add("ShowRegister", Configuration.AllowRegister);
                        parameters.Add("Error", e.Title ?? "An unknown error occured");
                    })
                );
            }

            return;
        }

        var code = await GenerateCode(model);

        var redirectUrl = redirectUri +
                          $"?code={code}";

        context.Response.Redirect(redirectUrl);
    }

    public async Task<TokenPair> HandleAccess(HttpContext context, string code)
    {
        await ValidateClientSecret(context);

        var userId = -1;

        // Check if the code is valid
        var isValidCode = TokenHelper.IsValidToken(
            code,
            Configuration.CodeSecret,
            parameters => parameters.TryGetValue("userId", out var userIdEl) && userIdEl.TryGetInt32(out userId)
        );

        if (!isValidCode)
            throw new HttpApiException("Invalid or expired code provided", 401);
        
        var implementation = context.RequestServices.GetRequiredService<ILocalProviderImplementation<T>>();
        var user = await implementation.LoadById(userId);

        if (user == null)
            throw new HttpApiException("Invalid user id in code", 400);

        return await GenerateAccess(user);
    }

    public async Task<TokenPair> HandleRefresh(HttpContext context, string refreshToken)
    {
        await ValidateClientSecret(context);
        
        var userId = -1;

        // Check if the refresh token is valid
        var isValidRefresh = TokenHelper.IsValidToken(
            refreshToken,
            Configuration.RefreshSecret,
            parameters => parameters.TryGetValue("userId", out var userIdEl) && userIdEl.TryGetInt32(out userId)
        );

        if (!isValidRefresh)
            throw new HttpApiException("Invalid or expired refresh token provided", 401);

        // Load user by the extracted id
        var implementation = context.RequestServices.GetRequiredService<ILocalProviderImplementation<T>>();
        var user = await implementation.LoadById(userId);

        if (user == null)
            throw new HttpApiException("Invalid user id in refresh token", 400);

        return await GenerateAccess(user);
    }

    public async Task<InfoResponse> Info(HttpContext context)
    {
        var user = await GetUser(context);

        return new InfoResponse()
        {
            Id = user.Id
        };
    }

    private async Task<T> GetUser(HttpContext context)
    {
        if (!context.Request.Headers.TryGetNotNull("Authorization", out var authHeader))
            throw new HttpApiException("Missing Authorization header", 401);

        var userId = -1;

        var isValid = TokenHelper.IsValidToken(authHeader, Configuration.AccessSecret, parameters
            => parameters.TryGetValue("userId", out var userIdEl) && userIdEl.TryGetInt32(out userId)
        );

        if (!isValid)
            throw new HttpApiException("Invalid access token", 401);

        var implementation = context.RequestServices.GetRequiredService<ILocalProviderImplementation<T>>();
        var user = await implementation.LoadById(userId);

        if (user == null)
            throw new HttpApiException("Invalid access token. User is not found", 400);

        return user;
    }

    private Task ValidateClientSecret(HttpContext context)
    {
        if (!context.Request.Headers.TryGetNotNull("Authorization", out var authHeader))
            throw new HttpApiException("Missing Authorization header", 401);

        if (authHeader != Configuration.ClientSecret)
            throw new HttpApiException("Invalid client secret provided", 400);
        
        return Task.CompletedTask;
    }

    private Task<TokenPair> GenerateAccess(T model)
    {
        var tokenPair = TokenHelper.GeneratePair(
            Configuration.AccessSecret,
            Configuration.RefreshSecret,
            parameters => { parameters.Add("userId", model.Id); },
            (int)Configuration.RefreshInterval.TotalSeconds,
            (int)Configuration.RefreshDuration.TotalSeconds
        );

        return Task.FromResult(tokenPair);
    }

    private Task<string> GenerateCode(T model)
    {
        var jwt = JwtHelper.Encode(Configuration.CodeSecret, parameters => { parameters.Add("userId", model.Id); },
            Configuration.RefreshInterval);

        return Task.FromResult(jwt);
    }
}