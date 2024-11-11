using Microsoft.AspNetCore.Builder;

namespace MoonCore.Extended.OAuth2.Consumer.Extensions;

public static class WebApplicationExtensions
{
    public static void UseOAuth2Authentication<T>(this WebApplication application) where T : IUserModel
    {
        application.MapGet("api/_auth/start", ControllerMethods.Start<T>);
        application.MapPost("api/_auth/complete", ControllerMethods.Complete<T>);
        application.MapPost("api/_auth/refresh", ControllerMethods.Refresh<T>);
    }
}