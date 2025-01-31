using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Extended.PermFilter;

namespace MoonCore.Blazor.Tailwind.Test.Http.Controllers;

[ApiController]
[Route("resource")]
public class ResourceController : Controller
{
    private readonly ILogger<ResourceController> Logger;

    public ResourceController(ILogger<ResourceController> logger)
    {
        Logger = logger;
    }

    [HttpGet]
    [Authorize]
    [RequirePermission("testy")]
    public async Task<string> Get()
    {
        Logger.LogInformation("Accessed resource using token");
        
        var claimString = "";

        foreach (var claim in User.Claims)
        {
            claimString += $"{claim.Type}: {claim.Value}\n";
        }
        
        return "Hello World\n" + claimString;
    }
}