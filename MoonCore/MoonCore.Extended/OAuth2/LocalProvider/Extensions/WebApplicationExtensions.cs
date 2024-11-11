using Microsoft.AspNetCore.Builder;
using MoonCore.Extended.OAuth2.Consumer;

namespace MoonCore.Extended.OAuth2.LocalProvider.Extensions;

public static class WebApplicationExtensions
{
    public static void UseLocalOAuth2Provider<T>(this WebApplication application) where T : IUserModel
    {
        application.MapGet("api/_auth/oauth2/authorize", ControllerMethods.Authorize<T>);
        application.MapPost("api/_auth/oauth2/authorize", ControllerMethods.AuthorizePost<T>).DisableAntiforgery();
        application.MapPost("api/_auth/oauth2/access", ControllerMethods.HandleAccess<T>).DisableAntiforgery();
        application.MapPost("api/_auth/oauth2/refresh", ControllerMethods.HandleRefresh<T>).DisableAntiforgery();
        application.MapGet("api/_auth/oauth2/info", ControllerMethods.HandleInfo<T>);

        application.UseMiddleware<AuthenticationMiddleware<T>>();
    }
}