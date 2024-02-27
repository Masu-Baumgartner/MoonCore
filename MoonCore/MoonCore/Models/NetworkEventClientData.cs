using System.Net.WebSockets;
using MoonCore.Helpers;

namespace MoonCore.Models;

public class NetworkEventClientData
{
    public WebSocket WebSocket { get; set; }
    public WsPacketConnection PacketConnection { get; set; }
}