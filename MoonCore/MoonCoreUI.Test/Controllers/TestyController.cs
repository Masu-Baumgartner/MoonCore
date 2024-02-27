using Microsoft.AspNetCore.Mvc;
using MoonCore.Helpers;
using MoonCoreUI.Test.Data;

namespace MoonCoreUI.Test.Controllers;

[ApiController]
[Route("testy")]
public class TestyController : ControllerBase
{
    private readonly NetworkService NetworkService;

    public TestyController(NetworkService networkService)
    {
        NetworkService = networkService;
    }

    [HttpGet]
    public async Task<ActionResult> Ws()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
            return BadRequest();
        
        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        var client = await NetworkService.Connection.AddClient(socket);

        await client.PacketConnection.WaitForClose();

        return Ok();
    }
}