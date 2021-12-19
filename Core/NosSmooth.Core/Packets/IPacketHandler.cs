using System.Threading;
using System.Threading.Tasks;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Calls registered responders for the packet that should be handled.
/// </summary>
public interface IPacketHandler
{
    /// <summary>
    /// Calls a responder for the given packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> HandleReceivedPacketAsync(IPacket packet, CancellationToken ct = default);

    /// <summary>
    /// Calls a responder for the given packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> HandleSentPacketAsync(IPacket packet, CancellationToken ct = default);
}