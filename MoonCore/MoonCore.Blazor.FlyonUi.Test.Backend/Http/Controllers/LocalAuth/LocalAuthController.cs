using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoonCore.Blazor.FlyonUi.Test.Backend.LocalAuth;
using MoonCore.Helpers;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers.LocalAuth;

[ApiController]
[Route("api/localauth")]
public class LocalAuthController : Controller
{
    private readonly IServiceProvider ServiceProvider;
    private readonly IAuthenticationService AuthenticationService;
    private readonly IOptionsMonitor<LocalAuthOptions> Options;

    public LocalAuthController(
        IServiceProvider serviceProvider,
        IOptionsMonitor<LocalAuthOptions> options, IAuthenticationService authenticationService)
    {
        ServiceProvider = serviceProvider;
        Options = options;
        AuthenticationService = authenticationService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IResult> Get()
    {
        var html = await ComponentHelper.RenderToHtmlAsync<LoginByEmailPage>(
            ServiceProvider,
            parameters => { parameters["IsEmailSent"] = false; });

        return Results.Content(html, "text/html");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IResult> Post(
        [FromForm(Name = "email")] string email
    )
    {
        Console.WriteLine("Invite URL:");
        Console.WriteLine(
            $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/localauth/handle?email={email}");

        var html = await ComponentHelper.RenderToHtmlAsync<LoginByEmailPage>(
            ServiceProvider,
            parameters => { parameters["IsEmailSent"] = true; });

        return Results.Content(html, "text/html");
    }

    [HttpGet("handle")]
    [AllowAnonymous]
    public async Task<IResult> Handle(
        [FromQuery(Name = "email")] string email
    )
    {
        var username = email.Split('@').First();
        var id = Random.Shared.Next(0, 9999);
        
        var options = Options.Get("LocalAuth");

        await AuthenticationService.SignInAsync(HttpContext, options.SignInScheme, new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, username)
                ],
                "LocalAuth"
            )
        ), new AuthenticationProperties());

        return Results.Redirect("/");
    }
}