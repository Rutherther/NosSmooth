using NosCore.Packets;
using NosCore.Packets.ServerPackets.MiniMap;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using Remora.Results;

namespace PacketInterceptor;

public class DummyNostaleClient : BaseNostaleClient
{
    private readonly IPacketHandler _packetHandler;

    public DummyNostaleClient(CommandProcessor commandProcessor, IPacketSerializer packetSerializer,
        IPacketHandler packetHandler) : base(commandProcessor, packetSerializer)
    {
        _packetHandler = packetHandler;
    }

    public override async Task<Result> RunAsync(CancellationToken stopRequested = default)
    {
        await _packetHandler.HandleSentPacketAsync(new CMapPacket()
            { Header = "t", Id = 2, IsValid = true, KeepAliveId = 123, MapType = true, Type = 0 }, stopRequested);
        return Result.FromSuccess();
    }

    public override Task<Result> SendPacketAsync(string packetString, CancellationToken ct = default)
    {
        Console.WriteLine($"Sending packet {packetString}");
        return Task.FromResult(Result.FromSuccess());
    }

    public override Task<Result> ReceivePacketAsync(string packetString, CancellationToken ct = default)
    {
        Console.WriteLine($"Receiving packet {packetString}");
        return Task.FromResult(Result.FromSuccess());
    }
}