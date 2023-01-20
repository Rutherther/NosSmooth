//
//  CInfoPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Character;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
public class CInfoPacketResponder : IPacketResponder<CInfoPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="CInfoPacketResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public CInfoPacketResponder(PathfinderState state)
    {
        _state = state;

    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<CInfoPacket> packetArgs, CancellationToken ct = default)
    {
        _state.CharacterId = packetArgs.Packet.CharacterId;
        return Task.FromResult(Result.FromSuccess());
    }
}