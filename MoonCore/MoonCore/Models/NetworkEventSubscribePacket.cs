namespace MoonCore.Models;

public class NetworkEventSubscribePacket
{
    public string Topic { get; set; }
    public TimeSpan TimeSpan { get; set; }
}