using System.Text.Json;
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

    private Task Authenticate(HttpContext context)
    {
        var request = context.Request;

        // Load token from header
        if (!request.Headers.TryGetValue("Authorization", out var authHeaderStrVal))
            return Task.CompletedTask;

        if (authHeaderStrVal.Count == 0)
            return Task.CompletedTask;

        var authHeader = authHeaderStrVal.First();

        // Validate format
        if (string.IsNullOrEmpty(authHeader))
            return Task.CompletedTask;

        if (!authHeader.StartsWith("Bearer"))
            return Task.CompletedTask;

        var authParts = authHeader.Split(" ");

        if (authParts.Length != 2)
            return Task.CompletedTask;

        var accessToken = authParts[1];

        // Validate access token
        var configuration = context.RequestServices.GetRequiredService<TokenAuthenticationConfig>();

        TokenHelper.IsValidAccessToken(accessToken, configuration.AccessSecret, data => configuration.ProcessAccess.Invoke(
            data,
            context.RequestServices,
            context
        ).Result);
        
        return Task.CompletedTask;
    }
}