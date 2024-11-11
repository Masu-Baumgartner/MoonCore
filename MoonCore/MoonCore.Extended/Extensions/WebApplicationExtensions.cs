using Microsoft.AspNetCore.Builder;
using MoonCore.Extended.Middleware;

namespace MoonCore.Extended.Extensions;

public static class WebApplicationExtensions
{
    public static void UseApiErrorHandling(this WebApplication application)
    {
        application.UseMiddleware<ApiErrorHandleMiddleware>();
    }
}