using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.Helpers.OAuth2;
using MoonCore.Extended.Middleware;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Extended.OAuth2.Provider;
using MoonCore.Http.Requests.OAuth2;
using MoonCore.Http.Requests.TokenAuthentication;

namespace MoonCore.Extended.Extensions;

public static class WebApplicationExtensions
{
    public static void UseApiErrorHandling(this WebApplication application)
    {
        application.UseMiddleware<ApiErrorHandleMiddleware>();
    }
    
    public static void UseTokenAuthentication(this WebApplication application)
    {
        application.MapPost("api/_auth/refresh", (
                [FromBody] RefreshRequest request,
                TokenAuthenticationConfig configuration,
                IServiceProvider serviceProvider
            )
            => AuthControllerHelper.Refresh(
                request,
                configuration,
                serviceProvider
            ).Result
        );

        application.UseMiddleware<TokenAuthenticationMiddleware>();
    }

    public static void UseOAuth2Consumer(this WebApplication application)
    {
        application.MapGet("api/_auth/oauth2/start", (OAuth2ConsumerService oAuth2Service)
            => OAuth2ControllerHelper.Start(oAuth2Service).Result
        );

        application.MapPost("api/_auth/oauth2/complete", (
                [FromBody] OAuth2CompleteRequest request,
                OAuth2ConsumerService oAuth2Service,
                IServiceProvider serviceProvider,
                OAuth2ConsumerConfiguration config,
                TokenAuthenticationConfig authenticationConfig
            )
            => OAuth2ControllerHelper.Complete(request, oAuth2Service, serviceProvider, config, authenticationConfig).Result
        );
    }

    public static void UseOAuth2LocalProvider(
        this WebApplication application,
        Action<LocalOAuth2ProviderConfig> onConfigure
    )
    {
        var config = new LocalOAuth2ProviderConfig();
        onConfigure.Invoke(config);

        application.MapGet("/api/_auth/oauth2/authorize", async Task (
            HttpContext context,
            OAuth2ProviderService providerService,
            [FromQuery(Name = "response_type")] string responseType,
            [FromQuery(Name = "client_id")] string clientId,
            [FromQuery(Name = "redirect_uri")] string redirectUri,
            [FromQuery(Name = "action")] string action = "login") =>
        {
            await LocalOAuth2ProviderControllerHelper.Authorize(
                context,
                providerService,
                config,
                responseType,
                clientId,
                redirectUri,
                action
            );
        });

        application.MapPost("/api/_auth/oauth2/authorize", async Task (
            HttpContext context,
            OAuth2ProviderService providerService,
            [FromQuery(Name = "response_type")] string responseType,
            [FromQuery(Name = "client_id")] string clientId,
            [FromQuery(Name = "redirect_uri")] string redirectUri,
            [FromForm(Name = "email")] string email,
            [FromForm(Name = "password")] string password,
            [FromForm(Name = "username")] string username = "",
            [FromQuery(Name = "action")] string action = "login") =>
        {
            await LocalOAuth2ProviderControllerHelper.AuthorizePost(
                context,
                providerService,
                config,
                email,
                username,
                password,
                responseType,
                clientId,
                redirectUri,
                action
            );
        });

        application.MapPost("/api/_auth/oauth2/access", async (
            OAuth2ProviderService providerService,
            [FromForm(Name = "client_id")] string clientId,
            [FromForm(Name = "client_secret")] string clientSecret,
            [FromForm(Name = "redirect_uri")] string redirectUri,
            [FromForm(Name = "grant_type")] string grantType,
            [FromForm(Name = "code")] string code
        ) =>
        {
            return await LocalOAuth2ProviderControllerHelper.Access(
                providerService,
                config,
                clientId,
                clientSecret,
                redirectUri,
                grantType,
                code
            );
        });
        
        application.MapPost("/api/_auth/oauth2/refresh", async (
            OAuth2ProviderService providerService,
            [FromForm(Name = "grant_type")] string grantType,
            [FromForm(Name = "refresh_token")] string refreshToken
        ) =>
        {
            return await LocalOAuth2ProviderControllerHelper.Refresh(
                providerService,
                config,
                grantType,
                refreshToken
            );
        });
        
        application.MapGet("/api/_auth/oauth2/info", async (
            HttpContext context,
            OAuth2ProviderService providerService
        ) =>
        {
            return await LocalOAuth2ProviderControllerHelper.Info(
                context,
                providerService,
                config
            );
        });
    }
}