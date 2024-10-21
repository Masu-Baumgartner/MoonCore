using Microsoft.AspNetCore.Builder;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.Middleware;

namespace MoonCore.Extended.Extensions;

public static class WebHostBuilderExtensions
{
    public static void UseTokenAuthentication(this IApplicationBuilder application)
    {
        application.UseMiddleware<TokenAuthenticationMiddleware>();
    }
}