using System.Collections.Frozen;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MoonCore.Extended.JwtInvalidation;

public class JwtInvalidateMiddleware
{
    private readonly ILogger<JwtInvalidateMiddleware> Logger;
    private readonly FrozenDictionary<string, IJwtInvalidateOptions> Options;
    private readonly RequestDelegate Next;

    public JwtInvalidateMiddleware(
        IEnumerable<IJwtInvalidateOptions> options,
        RequestDelegate next,
        ILogger<JwtInvalidateMiddleware> logger
    )
    {
        Next = next;
        Logger = logger;

        Options = options
            .ToFrozenDictionary(x => x.Scheme, x => x);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (await Handle(context)) // If we evaluate the current identity as invalidated, clear the identity
        {
            await Results.Unauthorized().ExecuteAsync(context);
            return;
        }

        await Next(context);
    }

    private async Task<bool> Handle(HttpContext context)
    {
        var isAnonymous = (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() ?? null) != null;

        if (isAnonymous)
            return false;

        // Only handle authenticated requests
        if (!(context.User.Identity?.IsAuthenticated ?? false))
            return false;

        // Only handle defined schemes
        var authenticateResultFeature = context.Features.Get<IAuthenticateResultFeature>();

        // Null checks
        if (authenticateResultFeature == null ||
            authenticateResultFeature.AuthenticateResult == null ||
            authenticateResultFeature.AuthenticateResult.Ticket == null)
        {
            return false;
        }

        // Lookup options for scheme
        var optionsForScheme = Options
            .GetValueOrDefault(authenticateResultFeature.AuthenticateResult.Ticket.AuthenticationScheme);
        
        // Don't handle if no options for this scheme have been found
        if (optionsForScheme == null)
            return false;

        // Invoke defined provider
        var invalidateTime = await optionsForScheme.InvalidateTimeProvider.Invoke(
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

        // When the token has been issued AFTER or AT we had our last invalidation, we can proceed
        if (issuedAtTime >= invalidateTime)
            return false;

        // Otherwise nuke the identity
        return true;
    }
}