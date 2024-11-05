using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.Helpers;
using MoonCore.Extended.Interfaces;
using MoonCore.Extended.Middleware;
using MoonCore.Extended.OAuth2.Consumer;
using MoonCore.Http.Requests.OAuth2;
using MoonCore.Http.Requests.TokenAuthentication;

namespace MoonCore.Extended.Extensions;

public static class WebApplicationExtensions
{
    public static void UseTokenAuthentication(this WebApplication application)
    {
        application.MapPost("_auth/refresh", (
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
        application.MapGet("_auth/oauth2/start", (OAuth2ConsumerService oAuth2Service)
            => OAuth2ControllerHelper.Start(oAuth2Service).Result
        );

        application.MapPost("_auth/oauth2/complete", (
                [FromBody] OAuth2CompleteRequest request,
                OAuth2ConsumerService oAuth2Service,
                IServiceProvider serviceProvider,
                IOAuth2Provider provider,
                TokenAuthenticationConfig config
            )
            => OAuth2ControllerHelper.Complete(request, oAuth2Service, serviceProvider, provider, config).Result
        );
    }
}