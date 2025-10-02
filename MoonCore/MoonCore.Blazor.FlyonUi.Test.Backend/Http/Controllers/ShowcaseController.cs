using Microsoft.AspNetCore.Mvc;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers;

[ApiController]
[Route("api/showcase")]
public class ShowcaseController : Controller
{
    private readonly string RootDirectory = Path.Combine(
        "..",
        "MoonCore.Blazor.FlyonUi.Test.Frontend",
        "UI",
        "Showcases"
    );

    [HttpGet("{name}")]
    public async Task<ActionResult> GetAsync([FromRoute] string name)
    {
        var fullPath = Path.Combine(RootDirectory, $"{name}.razor");

        if (!System.IO.File.Exists(fullPath))
            return Problem("No showcase with that name found", statusCode: 404);

        var contentStream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return File(contentStream, "text/plain");
    }
}