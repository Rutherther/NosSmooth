//
//  RestResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Server.UI;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responds to <see cref="RestPacket"/>.
/// </summary>
public class RestResponder : IPacketResponder<RestPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public RestResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<RestPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var entity = _game.CurrentMap?.Entities.GetEntity<ILivingEntity>(packet.EntityId);

        if (entity is not null)
        {
            entity.IsSitting = packet.IsSitting;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}