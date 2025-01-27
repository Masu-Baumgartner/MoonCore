using Microsoft.AspNetCore.Mvc;

namespace MoonCore.Blazor.Tailwind.Test.Http.Controllers;

[ApiController]
[Route("oauth2")]
public class OAuth2Controller : Controller
{
    private readonly ILogger<OAuth2Controller> Logger;

    public OAuth2Controller(ILogger<OAuth2Controller> logger)
    {
        Logger = logger;
    }

    [HttpGet("start")]
    public async Task Start([FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "redirect_uri")] string redirectUri)
    {
        Response.StatusCode = 200;
        await Response.WriteAsync(await System.IO.File.ReadAllTextAsync("oauth2.html"));
    }

    [HttpPost("start")]
    public async Task CompleteStart([FromQuery(Name = "client_id")] string clientId, [FromQuery(Name = "redirect_uri")] string redirectUri)
    {
        Logger.LogInformation("OAuth2 Start | Client id: {clientId}, Redirect uri: {redirectUri}", clientId,
            redirectUri);

        Response.Redirect(redirectUri + "?code=12345678");
    }
}