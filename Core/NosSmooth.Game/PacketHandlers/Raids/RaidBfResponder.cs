//
//  RaidBfResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Raids;
using NosSmooth.Packets.Enums.Raids;
using NosSmooth.Packets.Server.Raids;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Raids;

/// <summary>
/// A responder to <see cref="RaidBfPacket"/>.
/// </summary>
public class RaidBfResponder : IPacketResponder<RaidBfPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidBfResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public RaidBfResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RaidBfPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        Raid? previousRaid = null;
        var currentRaid = await _game.UpdateRaidAsync
        (
            raid =>
            {
                previousRaid = raid;
                return raid with
                {
                    State = packet.WindowType switch
                    {
                        RaidBfPacketType.MissionStarted => RaidState.Started,
                        RaidBfPacketType.MissionCleared => RaidState.EndedSuccessfully,
                        _ => RaidState
                            .TeamFailed // TODO: figure out whether OutOfLives is sent for both individual member and whole team
                    }
                };
            },
            ct: ct
        );

        if (previousRaid is not null && currentRaid is not null && previousRaid.State != currentRaid.State)
        {
            return await _eventDispatcher.DispatchEvent
                (new RaidStateChangedEvent(previousRaid.State, currentRaid.State, currentRaid), ct);
        }

        return Result.FromSuccess();
    }
}