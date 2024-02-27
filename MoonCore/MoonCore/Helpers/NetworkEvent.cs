namespace MoonCore.Helpers;

public class NetworkEvent<T>
{
    private readonly NetworkEventConnection Connection;
    private readonly string Topic;
    
    private DateTime SubscribedUntil = DateTime.MinValue;

    public NetworkEvent(string topic, NetworkEventConnection connection)
    {
        Topic = topic;
        Connection = connection;

        Connection.OnSubscribePacket += packet =>
        {
            SubscribedUntil = DateTime.UtcNow.Add(packet.TimeSpan);
            
            return Task.CompletedTask;
        };
    }

    public async Task Emit(T data)
    {
        if(DateTime.UtcNow > SubscribedUntil)
            return;
        
        await Connection.SendEmit(Topic, data!);
    }
}