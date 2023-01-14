//
//  RaidMbfResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Packets.Server.Raids;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Raids;

/// <summary>
/// A responder to <see cref="RaidMbfPacket"/>.
/// </summary>
public class RaidMbfResponder : IPacketResponder<RaidMbfPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidMbfResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public RaidMbfResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RaidMbfPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        await _game.UpdateRaidAsync
        (
            raid => raid with
            {
                Progress = new RaidProgress
                (
                    packet.MonsterLockerInitial,
                    packet.MonsterLockerCurrent,
                    packet.ButtonLockerInitial,
                    packet.ButtonLockerCurrent,
                    packet.CurrentLives,
                    packet.InitialLives
                )
            },
            ct: ct
        );
        return Result.FromSuccess();
    }
}