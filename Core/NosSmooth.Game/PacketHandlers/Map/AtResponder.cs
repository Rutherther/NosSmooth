//
//  AtResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Characters;
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
    public async Task<Result> Respond(PacketEventArgs<AtPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        await _game.CreateOrUpdateCharacterAsync
        (
            () => new Character()
            {
                Id = packet.CharacterId,
                Position = new Position(packet.X, packet.Y)
            },
            c =>
            {
                c.Position = new Position(packet.X, packet.Y);
                return c;
            },
            ct: ct
        );

        return Result.FromSuccess();
    }
}