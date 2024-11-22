using Microsoft.AspNetCore.Builder;

namespace MoonCore.Extended.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseApiExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseExceptionHandler();
    }
}