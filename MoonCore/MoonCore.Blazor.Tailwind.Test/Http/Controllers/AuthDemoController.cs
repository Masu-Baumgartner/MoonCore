using Microsoft.AspNetCore.Mvc;
using MoonCore.Helpers;

namespace MoonCore.Blazor.Tailwind.Test.Http.Controllers;

[ApiController]
[Route("authDemo")]
public class AuthDemoController : Controller
{
    private readonly ILogger<AuthDemoController> Logger;
    private readonly IServiceProvider ServiceProvider;

    public AuthDemoController(
        ILogger<AuthDemoController> logger,
        IServiceProvider serviceProvider
    )
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    [HttpGet]
    public async Task Get()
    {
        var content = await ComponentHelper.RenderComponent<AuthDemoPage>(
            ServiceProvider
        );

        HttpContext.Response.StatusCode = 200;
        await HttpContext.Response.WriteAsync(content);
    }

    [HttpPost("user")]
    public Task RegularLogin()
    {
        Response.Redirect("/?auth=user");
        
        return Task.CompletedTask;
    }
    
    [HttpPost("admin")]
    public Task AdminLogin()
    {
        Response.Redirect("/?auth=admin");
        
        return Task.CompletedTask;
    }
}