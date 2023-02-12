//
//  PtctlResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Client.Mates;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responds to Ptctl packet.
/// </summary>
public class PtctlResponder : IPacketResponder<PtctlPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="PtctlResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    public PtctlResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<PtctlPacket> packetArgs, CancellationToken ct = default)
    {
        var map = _game.CurrentMap;

        if (map is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        var packet = packetArgs.Packet;

        foreach (var control in packet.Controls)
        {
            var entity = map.Entities.GetEntity<ILivingEntity>(control.MateTransportId);
            if (entity is not null)
            {
                entity.Position = new Position(control.PositionX, control.PositionY);

                if (packet.EntityId == control.MateTransportId)
                {
                    entity.Speed = packet.Speed;
                }
            }
        }

        return Task.FromResult(Result.FromSuccess());
    }
}