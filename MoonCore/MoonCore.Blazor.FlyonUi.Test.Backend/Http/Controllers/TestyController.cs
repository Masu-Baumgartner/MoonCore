using Microsoft.AspNetCore.Mvc;
using MoonCore.Extended.Models;

namespace MoonCore.Blazor.FlyonUi.Test.Backend.Http.Controllers;

[ApiController]
[Route("api/testy")]
public class TestyController : Controller
{
    [HttpGet]
    public async Task<string> Get([FromRoute] PagedOptions options)
    {
        return $"{options.Page} {options.PageSize}";
    }
}