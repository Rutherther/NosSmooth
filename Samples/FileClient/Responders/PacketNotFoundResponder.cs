//
//  PacketNotFoundResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using Remora.Results;

namespace FileClient.Responders;

/// <summary>
/// Responds to packets that were not found.
/// </summary>
public class PacketNotFoundResponder : IPacketResponder<UnresolvedPacket>, IPacketResponder<ParsingFailedPacket>
{
    private readonly ILogger<PacketNotFoundResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketNotFoundResponder"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public PacketNotFoundResponder(ILogger<PacketNotFoundResponder> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<UnresolvedPacket> packetArgs, CancellationToken ct = default)
    {
        _logger.LogWarning($"Could not find packet {packetArgs.PacketString}");
        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<ParsingFailedPacket> packetArgs, CancellationToken ct = default)
    {
        _logger.LogWarning($"Could not parse packet {packetArgs.PacketString}");
        _logger.LogResultError(packetArgs.Packet.SerializerResult);
        return Task.FromResult(Result.FromSuccess());
    }
}