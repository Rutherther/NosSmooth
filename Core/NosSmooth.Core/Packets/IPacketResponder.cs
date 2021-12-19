using System.Threading;
using System.Threading.Tasks;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

public interface IPacketResponder
{
}

public interface IPacketResponder<TPacket> : IPacketResponder
    where TPacket : IPacket
{
    /// <summary>
    /// Respond to the given packet.
    /// </summary>
    /// <param name="packet">The packet to respond to.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns></returns>
    public Task<Result> Respond(TPacket packet, CancellationToken ct = default);
}

public interface IEveryPacketResponder : IPacketResponder
{
    public Task<Result> Respond<TPacket>(TPacket packet, CancellationToken ct = default)
        where TPacket : IPacket;
}