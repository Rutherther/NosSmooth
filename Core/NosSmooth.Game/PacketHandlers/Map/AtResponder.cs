//
//  AtResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responds to at packet.
/// </summary>
public class AtResponder : IPacketResponder<AtPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public AtResponder(Game game)
    {
        _game = game;

    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<AtPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        var entity = map.Entities.GetEntity(packet.CharacterId);
        if (entity is not null)
        {
            entity.Position = new Position(packet.X, packet.Y);
        }

        return Task.FromResult(Result.FromSuccess());
    }
}