using System.Net.WebSockets;
using MoonCore.Helpers;
using MoonCore.Test;

Logger.Setup(isDebug: true);

var clientWs = new ClientWebSocket();
await clientWs.ConnectAsync(new Uri("ws://localhost:5171/testy"), CancellationToken.None);

var aws = new AdvancedWebsocketStream(clientWs);

aws.RegisterPacket<WeatherForecast>(1);

for (int i = 0; i <= 100; i++)
{
    await aws.SendPacket(new WeatherForecast()
    {
        Summary = Formatter.GenerateString(10324),
        Date = DateOnly.MinValue,
        TemperatureC = 189289
    });
}

await Task.Delay(-1);