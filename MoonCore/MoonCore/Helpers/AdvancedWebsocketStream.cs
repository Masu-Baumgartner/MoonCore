using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace MoonCore.Helpers;

public class AdvancedWebsocketStream
{
    private readonly WebSocket Socket;
    private readonly int MaxBufferSize;
    
    private readonly Dictionary<int, Type> Packets = new();

    public AdvancedWebsocketStream(WebSocket socket, int maxBufferSize = 10 * 1024)
    {
        Socket = socket;
        MaxBufferSize = maxBufferSize;
    }

    public void RegisterPacket<T>(int id) => RegisterPacket(id, typeof(T));

    public void RegisterPacket(int id, Type type)
    {
        Packets.Add(id, type);
    }

    public async Task<object?> ReceivePacket()
    {
        if (Socket.State != WebSocketState.Open)
            throw new ArgumentException("The websocket connection needs to be open in order to receive packets");

        // Length
        var lengthBuffer = new byte[4];
        await Socket.ReceiveAsync(lengthBuffer, CancellationToken.None);
        var length = BitConverter.ToInt32(lengthBuffer);

        // Validate length
        if (length <= 0)
            throw new ArgumentException("The packet length cannot be less or equal than zero");

        if (length > MaxBufferSize)
            throw new ArgumentException("The packet lenght cannot be greater than the max buffer size");

        // Packet
        var packetBuffer = new byte[length];
        await Socket.ReceiveAsync(packetBuffer, CancellationToken.None);

        return DecodePacket(packetBuffer);
    }

    public async Task<T?> ReceivePacket<T>()
    {
        var packet = await ReceivePacket();

        if (packet == null)
            return default;
        
        if (packet is not T)
            throw new ArgumentException($"Received packet {packet.GetType().Name} matches not the type {typeof(T).Name}");

        return (T)packet;
    }

    public async Task SendPacket(object packet)
    {
        if (Socket.State != WebSocketState.Open)
            throw new ArgumentException("The websocket connection needs to be open in order to send packets");

        using var dataStream = new MemoryStream();

        // Encode packet
        var packetStream = EncodePacket(packet);

        // Write length
        dataStream.Write(BitConverter.GetBytes(packetStream.Length));

        // Copy packet stream to data stream
        packetStream.WriteTo(dataStream);
        await packetStream.DisposeAsync();
        
        await Socket.SendAsync(dataStream.ToArray(), WebSocketMessageType.Binary, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
    }
    
    public async Task WaitForClose()
    {
        var source = new TaskCompletionSource();

        Task.Run(async () =>
        {
            while (Socket.State == WebSocketState.Open)
                await Task.Delay(10);
            
            source.SetResult();
        });

        await source.Task;
    }
    
    public async Task Close()
    {
        if(Socket.State == WebSocketState.Open) 
            await Socket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
    }

    private MemoryStream EncodePacket(object packet)
    {
        var type = packet.GetType();

        var packetId = Packets.Values.Contains(type) ? Packets.First(x => x.Value == type).Key : -1;

        if (packetId == -1)
            throw new ArgumentException($"Sending packet type which has not been registered: {packet.GetType().Name}");

        var memoryStream = new MemoryStream();
        
        // Header
        memoryStream.Write(BitConverter.GetBytes(packetId));

        // Body
        var jsonText = JsonConvert.SerializeObject(packet);
        memoryStream.Write(Encoding.UTF8.GetBytes(jsonText));

        return memoryStream;
    }

    private object? DecodePacket(byte[] buffer)
    {
        if (buffer.Length < 5) // 4 (header) + minimum 1 as body
        {
            Logger.Warn($"Received buffer is too small ({buffer.Length} bytes)");
            return default;
        }

        var headerBuffer = new byte[4];
        Array.Copy(buffer, 0, headerBuffer, 0, 4);
        var packetId = BitConverter.ToInt32(headerBuffer);

        var packetType = Packets.TryGetValue(packetId, out var packet) ? packet : default;
        
        if (packetType == null)
        {
            Logger.Warn($"Received packet id which has not been registered: {packetId}");
            return default;
        }

        var bodyBuffer = new byte[buffer.Length - 4];
        Array.Copy(buffer, 4, bodyBuffer, 0, buffer.Length - 4);

        var jsonText = Encoding.UTF8.GetString(bodyBuffer);

        if (string.IsNullOrEmpty(jsonText))
        {
            Logger.Warn("Received empty json text");
            return default;
        }

        object? result = default;
        
        try
        {
            result = JsonConvert.DeserializeObject(jsonText, packetType);
        }
        catch (JsonReaderException e)
        {
            Logger.Warn($"An error occured while deserializating the json text of the packet {packetType.Name}");
            Logger.Warn(e);
        }

        return result;
    }
}