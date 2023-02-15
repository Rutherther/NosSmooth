//
//  PtctlResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security;
using NosSmooth.Core.Packets;
using NosSmooth.Packets.Client.Mates;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
public class PtctlResponder : IPacketResponder<PtctlPacket>
{
    private readonly PathfinderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="PtctlResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    public PtctlResponder(PathfinderState state)
    {
        _state = state;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<PtctlPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        foreach (var walkControl in packet.Controls)
        {
            if (!_state.Entities.TryGetValue(walkControl.MateTransportId, out var entityState))
            {
                _state.AddEntity(walkControl.MateTransportId, walkControl.PositionX, walkControl.PositionY);
                continue;
            }

            entityState.X = walkControl.PositionX;
            entityState.Y = walkControl.PositionY;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}