using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MoonCore.Extended.JwtInvalidation;

public class JwtInvalidateMiddleware
{
    private readonly ILogger<JwtInvalidateMiddleware> Logger;
    private readonly JwtInvalidateOptions Options;
    private readonly RequestDelegate Next;

    public JwtInvalidateMiddleware(JwtInvalidateOptions options, RequestDelegate next, ILogger<JwtInvalidateMiddleware> logger)
    {
        Options = options;
        Next = next;
        Logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (await Handle(context)) // If we evaluate the current identity as invalidated, clear the identity
        {
            await context.ForbidAsync();
            return;
        }
        
        await Next(context);
    }

    private async Task<bool> Handle(HttpContext context)
    {
        // Only handle authenticated requests
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return false;

        // Invoke defined provider
        var invalidateTime = await Options.InvalidateTimeProvider.Invoke(
            context.RequestServices,
            context.User
        );

        // Do nothing when the provider does not return a date time
        if (!invalidateTime.HasValue)
            return false;

        // Now that we know we need to check the invalidate time, lets load the required claim
        var iatClaim = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat);

        // No issue at claim => We cant validate anything, proceed regular 
        if (iatClaim == null)
            return false;

        // Unable to parse issued at timestamp? => We cant validate anything, proceed regular
        if (!long.TryParse(iatClaim.Value, out var issuedAtUnix))
        {
            Logger.LogWarning("Unable to parse iat claim value. Value provided: '{value}'", iatClaim.Value);
            return false;
        }

        var issuedAtTime = DateTimeOffset.FromUnixTimeSeconds(issuedAtUnix).UtcDateTime;

        // When the token has been issued AFTER we had our last invalidation, we can proceed
        if (issuedAtTime > invalidateTime)
            return false;

        // Otherwise nuke the identity
        return true;
    }
}