using System.Net.WebSockets;
using MoonCore.Models;

namespace MoonCore.Helpers;

public class NetworkEventClientConnection
{
    public SmartEventHandler OnDisconnected { get; set; } = new();
    public SmartEventHandler<Exception> OnReceiveError { get; set; } = new();
    public SmartEventHandler<NetworkEventEmitPacket> OnEmitPacket { get; set; } = new();

    private readonly WsPacketConnection PacketConnection;
    private readonly WebSocket WebSocket;

    public NetworkEventClientConnection(WebSocket webSocket)
    {
        PacketConnection = new WsPacketConnection(webSocket);
        WebSocket = webSocket;

        PacketConnection.RegisterPacket<NetworkEventSubscribePacket>("subscribe");
        PacketConnection.RegisterPacket<NetworkEventEmitPacket>("emit");

        Task.Run(WorkTask);
    }

    private async Task WorkTask()
    {
        try
        {
            while (WebSocket.State == WebSocketState.Open)
            {
                var packet = await PacketConnection.Receive<NetworkEventEmitPacket>();

                await OnEmitPacket.Invoke(packet);
            }
        }
        catch (Exception e)
        {
            await OnReceiveError.Invoke(e);
        }

        await OnDisconnected.Invoke();
    }

    public async Task SendSubscription(string topic, TimeSpan timeSpan)
    {
        await PacketConnection.Send(new NetworkEventSubscribePacket()
        {
            Topic = topic,
            TimeSpan = timeSpan
        });
    }
}