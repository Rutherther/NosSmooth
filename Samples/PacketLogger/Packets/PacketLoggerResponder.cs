using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NosCore.Packets;
using NosCore.Packets.Interfaces;
using NosSmooth.Core.Packets;
using Remora.Results;

namespace PacketLogger.Packets;

public class PacketLoggerResponder : IEveryPacketResponder
{
    private readonly ILogger<PacketLoggerResponder> _logger;
    private readonly Serializer _serializer;

    public PacketLoggerResponder(ILogger<PacketLoggerResponder> logger, Serializer serializer)
    {
        _logger = logger;
        _serializer = serializer;
    }

    public Task<Result> Respond<TPacket>(TPacket packet, CancellationToken ct = default) where TPacket : IPacket
    {
        _logger.LogInformation(_serializer.Serialize(packet));
        return Task.FromResult(Result.FromSuccess());
    }
}