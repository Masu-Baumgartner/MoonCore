using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using MoonCore.Helpers;
using MoonCoreUI.Test.Data;

namespace MoonCoreUI.Test.Controllers;

[ApiController]
[Route("testy")]
public class TestyController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Ws()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
            return BadRequest();
        
        var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        var aws = new AdvancedWebsocketStream(socket);
        
        aws.RegisterPacket<WeatherForecast>(1);

        while (socket.State == WebSocketState.Open)
        {
            var packet = await aws.ReceivePacket();

            if (packet is WeatherForecast forecast)
            {
                Logger.Info(forecast.Summary);
                Logger.Info(forecast.Date.ToString());
                Logger.Info(forecast.TemperatureC.ToString());
            }
            else
            {
                Logger.Info(packet.GetType().ToString());
            }
        }

        await aws.WaitForClose();

        return Ok();
    }
}