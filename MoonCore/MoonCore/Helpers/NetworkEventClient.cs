using Newtonsoft.Json;

namespace MoonCore.Helpers;

public class NetworkEventClient<T>
{
    public SmartEventHandler<T> OnData { get; set; } = new();
    
    private readonly string Topic;
    private readonly NetworkEventClientConnection ClientConnection;

    public NetworkEventClient(string topic, NetworkEventClientConnection clientConnection)
    {
        Topic = topic;
        ClientConnection = clientConnection;

        ClientConnection.OnEmitPacket += async packet =>
        {
            if(packet.Topic != Topic)
                return;

            // We use base64 for the content instead of a string or putting the json into the object
            // as the json in json thing might break the packet parser from the ws packet connection
            // because the ws packet connection uses this method as well
            var json = Formatter.FromBase64ToText(packet.Content);

            var data = JsonConvert.DeserializeObject<T>(json);

            await OnData.Invoke(data);
        };
    }

    public async Task Subscribe(TimeSpan timeSpan) => await ClientConnection.SendSubscription(Topic, timeSpan);
}