using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace MoonCore.Extended.JwtInvalidation;

public static class JwtInvalidateExtensions
{
    public static void AddJwtBearerInvalidation(this IServiceCollection collection, string name = JwtBearerDefaults.AuthenticationScheme)
    {
        collection.PostConfigure<JwtBearerOptions>(name, options =>
        {
            if (options.EventsType == null)
                options.Events = new();
            
            options.Events.OnTokenValidated += async context =>
            {
                if(context.Principal == null)
                    return;
                
                var handlers = context
                    .HttpContext
                    .RequestServices
                    .GetRequiredService<IEnumerable<IJwtInvalidateHandler>>()
                    .ToArray();

                foreach (var handler in handlers)
                {
                    if (!await handler.Handle(context.Principal))
                        continue;
                    
                    context.Fail("Your jwt/session has been invalidated");
                    return;
                }
            };
        });
    }
}