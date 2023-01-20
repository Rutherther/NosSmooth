//
//  SuPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
public class SuPacketResponder : IPacketResponder<SuPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuPacketResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public SuPacketResponder(PathfinderState state)
    {
        _state = state;

    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<SuPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.CasterEntityId == _state.CharacterId)
        {
            if (packet.PositionX is null || packet.PositionY is null)
            {
                return Task.FromResult(Result.FromSuccess());
            }

            if (packet.PositionX == 0 || packet.PositionY == 0)
            {
                return Task.FromResult(Result.FromSuccess());
            }

            _state.X = packet.PositionX.Value;
            _state.Y = packet.PositionY.Value;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}