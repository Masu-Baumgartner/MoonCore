using Microsoft.AspNetCore.Mvc;

namespace MoonCore.Blazor.Tailwind.Test.Http.Controllers;

[ApiController]
[Route("api/testy")]
public class TestyController : Controller
{
    [HttpPost("upload")]
    public async Task Upload()
    {
        Console.WriteLine("Upload?????");
        
        foreach (var file in Request.Form.Files)
        {
            Console.WriteLine($"{file.FileName} {file.Length}");
        }
    }
}