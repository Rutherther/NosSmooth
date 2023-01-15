//
//  TpResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// A responder to <see cref="TpPacket"/>.
/// </summary>
public class TpResponder : IPacketResponder<TpPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="TpResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public TpResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<TpPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var entity = _game.CurrentMap?.Entities.GetEntity(packet.EntityId);

        if (entity is not null)
        {
            entity.Position = new Position(packet.PositionX, packet.PositionY);
        }

        return Task.FromResult(Result.FromSuccess());
    }
}