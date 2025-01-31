using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Extended.JwtInvalidation;

public static class JwtInvalidationExtensions
{
    public static void AddJwtInvalidation(this IServiceCollection serviceCollection, Action<JwtInvalidateOptions> onConfigure)
    {
        var options = new JwtInvalidateOptions();
        onConfigure.Invoke(options);

        serviceCollection.AddSingleton(options);
    }
    
    public static void UseJwtInvalidation(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<JwtInvalidateMiddleware>();
    }
}