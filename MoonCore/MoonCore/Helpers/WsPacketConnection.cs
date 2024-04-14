using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace MoonCore.Helpers;

/// <summary>
/// A abstraction layer on top of websockets to send and receive packets. Its very basic
/// </summary>
public class WsPacketConnection
{
    private readonly Dictionary<string, Type> Packets = new();
    private readonly WebSocket WebSocket;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webSocket">A websocket connection which is already connected and the abstraction should use to send and receive data</param>
    public WsPacketConnection(WebSocket webSocket)
    {
        WebSocket = webSocket;
    }

    /// <summary>
    /// Register a packet to the internal packet registry
    /// </summary>
    /// <param name="id">The id of the packet</param>
    /// <typeparam name="T">The type of the packet</typeparam>
    /// <returns></returns>
    public Task RegisterPacket<T>(string id)
    {
        lock (Packets)
            Packets.Add(id, typeof(T));

        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Register a packet to the internal packet registry
    /// </summary>
    /// <param name="id">The id of the packet</param>
    /// <param name="type">The type of a packet</param>
    /// <returns></returns>
    public Task RegisterPacket(string id, Type type)
    {
        lock (Packets)
            Packets.Add(id, type);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Send a packet through the websocket
    /// </summary>
    /// <param name="packet">A instance of the packet data class</param>
    /// <exception cref="ArgumentException">Will be thrown if the type of the packet is unknown to the registry</exception>
    public async Task Send(object packet)
    {
        string? packetId = null;

        // Search packet registration
        lock (Packets)
        {
            if (Packets.Any(x => x.Value == packet.GetType()))
                packetId = Packets.First(x => x.Value == packet.GetType()).Key;

            if (packetId == null)
                throw new ArgumentException($"A packet with the type {packet.GetType().FullName} is not registered");
        }

        // Build raw packet
        var rawPacket = new RawPacket()
        {
            Id = packetId,
            Data = packet
        };

        // Serialize, encode and build buffer
        var json = JsonConvert.SerializeObject(rawPacket);
        var buffer = Encoding.UTF8.GetBytes(json);

        await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, WebSocketMessageFlags.None,
            CancellationToken.None);
    }

    /// <summary>
    /// Receive a packet from tghe websocket connection. Will be null if the data received is not valid. Use type checks with "packet is MyNiceDataPacketClass" to check which type the packet is of
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Will be thrown if the received packet id is not known to the registry</exception>
    public async Task<object?> Receive(int bufferSize = 4096)
    {
        // Build buffer and read
        var buffer = new byte[bufferSize];
        await WebSocket.ReceiveAsync(buffer, CancellationToken.None);

        // Decode and deserialize
        var json = Encoding.UTF8.GetString(buffer);
        var rawPacket = JsonConvert.DeserializeObject<RawPacket>(json)!;

        object? packetType = null;

        // Search packet registration
        lock (Packets)
        {
            if (Packets.ContainsKey(rawPacket.Id))
                packetType = Packets[rawPacket.Id];

            if (packetType == null)
                throw new ArgumentException($"A packet with the type {rawPacket.Id} is not registered");
        }

        var typedPacketType = typeof(RawPacket<>).MakeGenericType((packetType as Type)!);
        var typedPacket = JsonConvert.DeserializeObject(json, typedPacketType);
        
        return typedPacketType.GetProperty("Data")!.GetValue(typedPacket);
    }

    /// <summary>
    /// Receive a specific packet. Will fail if the received packet type is not the expected
    /// </summary>
    /// <typeparam name="T">The type of the packet</typeparam>
    /// <returns></returns>
    public async Task<T?> Receive<T>(int bufferSize = 4096)
    {
        var o = await Receive(bufferSize);

        if (o == null)
            return default;
        
        return (T)o;
    }

    /// <summary>
    /// Closes the websocket connection in a clean ay
    /// </summary>
    public async Task Close()
    {
        if(WebSocket.State == WebSocketState.Open) 
            await WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
    }
    
    /// <summary>
    /// This method waits until the connection has been closed
    /// </summary>
    public async Task WaitForClose()
    {
        var source = new TaskCompletionSource();

        Task.Run(async () =>
        {
            while (WebSocket.State == WebSocketState.Open)
                await Task.Delay(10);
            
            source.SetResult();
        });

        await source.Task;
    }

    public class RawPacket
    {
        public string Id { get; set; }
        public object Data { get; set; }
    }

    public class RawPacket<T>
    {
        public string Id { get; set; }
        public T Data { get; set; }
    }
}