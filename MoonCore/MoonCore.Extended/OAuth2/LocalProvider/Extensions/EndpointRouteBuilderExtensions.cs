using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using MoonCore.Extended.OAuth2.Consumer;

namespace MoonCore.Extended.OAuth2.LocalProvider.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapLocalOAuth2Provider<T>(this IEndpointRouteBuilder routeBuilder) where T : IUserModel
    {
        routeBuilder.MapGet("api/_auth/oauth2/authorize", ControllerMethods.Authorize<T>);
        routeBuilder.MapPost("api/_auth/oauth2/authorize", ControllerMethods.AuthorizePost<T>).DisableAntiforgery();
        routeBuilder.MapPost("api/_auth/oauth2/access", ControllerMethods.HandleAccess<T>).DisableAntiforgery();
        routeBuilder.MapPost("api/_auth/oauth2/refresh", ControllerMethods.HandleRefresh<T>).DisableAntiforgery();
        routeBuilder.MapGet("api/_auth/oauth2/info", ControllerMethods.HandleInfo<T>);
    }
}