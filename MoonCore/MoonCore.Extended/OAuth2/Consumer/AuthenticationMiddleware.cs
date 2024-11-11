using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Authentication;
using MoonCore.Extensions;

namespace MoonCore.Extended.OAuth2.Consumer;

public class AuthenticationMiddleware<T> where T : IUserModel
{
    private readonly RequestDelegate Next;

    public AuthenticationMiddleware(RequestDelegate next)
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
        if (!request.Headers.TryGetNotNull("Authorization", out var authHeader))
            return;

        if (!authHeader.StartsWith("Bearer"))
            return;

        var authParts = authHeader.Split(" ");

        if (authParts.Length != 2)
            return;

        var accessToken = authParts[1];
        var consumerService = context.RequestServices.GetRequiredService<ConsumerService<T>>();
        var user = await consumerService.ValidateAccess(context.RequestServices, accessToken);
        
        if(user == null)
            return;

        context.User = new PermClaimsPrinciple()
        {
            IdentityModel = user
        };
    }
}