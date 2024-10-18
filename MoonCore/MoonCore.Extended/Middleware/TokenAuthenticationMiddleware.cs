using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Extended.Configuration;
using MoonCore.Extended.Helpers;

namespace MoonCore.Extended.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate Next;

    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await Authenticate(context);
        await Next(context);
    }

    private async Task Authenticate(HttpContext context)
    {
        var request = context.Request;

        // Load token from header
        if (!request.Headers.TryGetValue("Authorization", out var authHeaderStrVal))
            return;

        if (authHeaderStrVal.Count == 0)
            return;

        var authHeader = authHeaderStrVal.First();

        // Validate format
        if (string.IsNullOrEmpty(authHeader))
            return;

        if (!authHeader.StartsWith("Bearer"))
            return;

        var authParts = authHeader.Split(" ");

        if (authParts.Length != 2)
            return;

        var accessToken = authParts[1];

        // Validate access token

        var tokenHelper = context.RequestServices.GetRequiredService<TokenHelper>();
        var configuration = context.RequestServices.GetRequiredService<TokenAuthenticationConfiguration>();

        await tokenHelper.IsValidAccessToken(accessToken, configuration.AccessSecret, Validate);
        return;

        bool Validate(Dictionary<string, string> data)
        {
            return configuration.DataLoader.Invoke(
                data,
                context.RequestServices,
                context
            ).Result;
        }
    }
}