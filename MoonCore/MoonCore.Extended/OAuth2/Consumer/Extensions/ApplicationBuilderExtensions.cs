using Microsoft.AspNetCore.Builder;

namespace MoonCore.Extended.OAuth2.Consumer.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseOAuth2Authentication<T>(this IApplicationBuilder builder) where T : IUserModel
    {
        builder.UseMiddleware<AuthenticationMiddleware<T>>();
    }
}