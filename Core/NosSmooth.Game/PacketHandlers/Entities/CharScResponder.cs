//
//  CharScResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// A responder to <see cref="CharScPacket"/>.
/// </summary>
public class CharScResponder : IPacketResponder<CharScPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharScResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public CharScResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<CharScPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var entity = _game.CurrentMap?.Entities.GetEntity<ILivingEntity>(packet.EntityId);

        if (entity is not null)
        {
            entity.Size = packet.Size;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}