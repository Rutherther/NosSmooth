using NosCore.Packets.Interfaces;
using NosCore.Packets.ServerPackets.MiniMap;
using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using Remora.Results;

namespace PacketInterceptor;

public class TestResponder : IEveryPacketResponder
{
    private readonly INostaleClient _client;

    public TestResponder(INostaleClient client)
    {
        _client = client;
    }
    
    public async Task<Result> Respond<TPacket>(TPacket packet, CancellationToken ct = default) where TPacket : IPacket
    {
        return await _client.SendPacketAsync("test");
    }
}