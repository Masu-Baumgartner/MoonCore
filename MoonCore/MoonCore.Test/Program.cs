using System.Net.WebSockets;
using MoonCore.Helpers;

Logger.Setup();

var clientWebsocket = new ClientWebSocket();

await clientWebsocket.ConnectAsync(new Uri("ws://localhost:5171/testy"), CancellationToken.None);

var connection = new NetworkEventClientConnection(clientWebsocket);
var consoleMessage = new NetworkEventClient<string>("console.message", connection);

consoleMessage.OnData += (message) =>
{
    Logger.Info($"Console: {message}");
    
    return Task.CompletedTask;
};

Console.ReadLine();

await consoleMessage.Subscribe(TimeSpan.FromSeconds(10));

Console.ReadLine();