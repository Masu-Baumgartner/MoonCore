using System.Net.WebSockets;
using MoonCore.Models;
using Newtonsoft.Json;

namespace MoonCore.Helpers;

public class NetworkEventConnection
{
    public SmartEventHandler<NetworkEventSubscribePacket> OnSubscribePacket { get; set; } = new();
    public SmartEventHandler<Exception> OnReceiveError { get; set; } = new();
    public SmartEventHandler<Exception> OnEmitError { get; set; } = new();
    public SmartEventHandler<NetworkEventClientData> OnClientConnected { get; set; } = new();
    public SmartEventHandler<NetworkEventClientData> OnClientDisconnected { get; set; } = new();

    private readonly List<NetworkEventClientData> Clients = new();
    
    public async Task<NetworkEventClientData> AddClient(WebSocket webSocket)
    {
        var client = new NetworkEventClientData()
        {
            WebSocket = webSocket,
            PacketConnection = new WsPacketConnection(webSocket)
        };
            
        await client.PacketConnection.RegisterPacket<NetworkEventSubscribePacket>("subscribe");
        await client.PacketConnection.RegisterPacket<NetworkEventEmitPacket>("emit");
        
        lock (Clients)
        {
            Clients.Add(client);
        }

        Task.Run(async () =>
        {
            try
            {
                while (client.WebSocket.State == WebSocketState.Open)
                {
                    var packet = await client.PacketConnection.Receive<NetworkEventSubscribePacket>();

                    await OnSubscribePacket.Invoke(packet);
                }
            }
            catch (Exception e)
            {
                await OnReceiveError.Invoke(e);
            }

            lock (Clients)
            {
                if (Clients.Contains(client))
                    Clients.Remove(client);
            }
            
            await OnClientDisconnected.Invoke(client);
        });

        await OnClientConnected.Invoke(client);

        return client;
    }
    
    public async Task SendEmit(string topic, object data)
    {
        var json = JsonConvert.SerializeObject(data);
        var content = Formatter.FromTextToBase64(json);

        NetworkEventClientData[] clients;

        lock (Clients)
            clients = Clients.ToArray();
        
        foreach (var client in clients)
        {
            // Connection check
            if (client.WebSocket.State != WebSocketState.Open)
            {
                lock (Clients)
                {
                    if (Clients.Contains(client))
                        Clients.Remove(client);
                }
                
                await OnClientDisconnected.Invoke(client);
                
                continue;
            }

            // To ensure we have a non blocking call
            Task.Run(async () =>
            {
                try
                {
                    await client.PacketConnection.Send(new NetworkEventEmitPacket()
                    {
                        Topic = topic,
                        Content = content
                    });
                }
                catch (Exception e)
                {
                    await OnEmitError.Invoke(e);
                }
            });
        }
    }
}