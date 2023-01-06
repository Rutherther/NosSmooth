//
//  ReviveResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// A handler of <see cref="RevivePacket"/>.
/// </summary>
public class ReviveResponder : IPacketResponder<RevivePacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviveResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public ReviveResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<RevivePacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var entity = _game.CurrentMap?.Entities.GetEntity<ILivingEntity>(packet.EntityId);

        if (entity is not null)
        {
            var hp = entity.Hp;
            var mp = entity.Mp;
            if (hp is not null)
            {
                hp.Percentage = 100;
            }

            if (mp is not null)
            {
                mp.Percentage = 100;
            }
        }

        return Task.FromResult(Result.FromSuccess());
    }
}