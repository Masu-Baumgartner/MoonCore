using Microsoft.AspNetCore.Mvc;

namespace MoonCore.Blazor.Tailwind.Test.Controllers;

[ApiController]
[Route("api/cool")]
public class CoolController : Controller
{
    [HttpPost]
    public async Task Upload()
    {
        
    }
}