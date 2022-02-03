//
//  StatPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Server.Entities;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Characters;

/// <summary>
/// Responder to stat packet.
/// </summary>
public class StatPacketResponder : IPacketResponder<StatPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatPacketResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public StatPacketResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<StatPacket> packetArgs, CancellationToken ct = default)
    {
        var character = _game.Character;
        if (character is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        var packet = packetArgs.Packet;
        if (character.Hp is null)
        {
            character.Hp = new Health
            {
                Amount = packet.Hp,
                Maximum = packet.HpMaximum
            };
        }
        else
        {
            character.Hp.Amount = packet.Hp;
            character.Hp.Maximum = packet.HpMaximum;
        }

        if (character.Mp is null)
        {
            character.Mp = new Health
            {
                Amount = packet.Mp,
                Maximum = packet.MpMaximum
            };
        }
        else
        {
            character.Mp.Amount = packet.Mp;
            character.Mp.Maximum = packet.MpMaximum;
        }

        return Task.FromResult(Result.FromSuccess());
    }
}