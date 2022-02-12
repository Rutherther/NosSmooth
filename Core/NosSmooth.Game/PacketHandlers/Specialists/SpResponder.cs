//
//  SpResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Specialists;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Specialists;

/// <summary>
/// Responds to sp packet.
/// </summary>
public class SpResponder : IPacketResponder<SpPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public SpResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<SpPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var character = _game.Character;

        if (character is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        character.SpPoints = packet.SpPoints;
        character.AdditionalSpPoints = packet.AdditionalSpPoints;

        return Task.FromResult(Result.FromSuccess());
    }
}