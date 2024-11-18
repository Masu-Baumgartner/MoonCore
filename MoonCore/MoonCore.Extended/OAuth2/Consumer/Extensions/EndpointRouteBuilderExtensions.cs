using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MoonCore.Extended.OAuth2.Consumer.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapOAuth2Authentication<T>(this IEndpointRouteBuilder routeBuilder) where T : IUserModel
    {
        routeBuilder.MapGet("api/_auth/start", ControllerMethods.Start<T>);
        routeBuilder.MapPost("api/_auth/complete", ControllerMethods.Complete<T>);
        routeBuilder.MapPost("api/_auth/refresh", ControllerMethods.Refresh<T>);
    }
}