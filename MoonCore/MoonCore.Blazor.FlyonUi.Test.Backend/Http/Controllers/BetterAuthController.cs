using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Blazor.FlyonUi.Test.Shared.Http.Responses;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers;

[ApiController]
[Route("api/auth")]
public class BetterAuthController : Controller
{
    private readonly IAuthenticationSchemeProvider SchemeProvider;

    public BetterAuthController(IAuthenticationSchemeProvider schemeProvider)
    {
        SchemeProvider = schemeProvider;
    }

    [HttpGet]
    public async Task<AuthSchemeResponse[]> GetSchemes()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();

        return schemes
            .Where(x => x.DisplayName != null)
            .Select(scheme => new AuthSchemeResponse()
            {
                DisplayName = scheme.DisplayName!,
                Identifier = scheme.Name
            })
            .ToArray();
    }

    [HttpGet("{identifier:alpha}")]
    public async Task StartScheme([FromRoute] string identifier)
    {
        var scheme = await SchemeProvider.GetSchemeAsync(identifier);

        // The check for the display name ensures a user isn't starting an auth flow
        // which isn't meant for users
        if (scheme == null || scheme.DisplayName == null) 
        {
            await Results.BadRequest("Invalid identifier").ExecuteAsync(HttpContext);
            return;
        }

        await HttpContext.ChallengeAsync(
            scheme.Name,
            new AuthenticationProperties()
            {
                RedirectUri = "/"
            }
        );
    }

    [Authorize]
    [HttpGet("check")]
    public async Task<Dictionary<string, string>> Check()
    {
        var username = HttpContext.User.FindFirstValue(ClaimTypes.Name)!;
        var id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;

        var claims = new Dictionary<string, string>
        {
            { ClaimTypes.Name, username },
            { ClaimTypes.NameIdentifier, id },
            { ClaimTypes.Email, email }
        };

        return claims;
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
        await Results.Redirect("/").ExecuteAsync(HttpContext);
    }
}