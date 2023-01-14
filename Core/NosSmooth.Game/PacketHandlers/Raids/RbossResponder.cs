//
//  RbossResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Raids;
using NosSmooth.Packets.Server.Raids;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Raids;

/// <summary>
/// A responder to <see cref="RbossPacket"/>.
/// </summary>
public class RbossResponder : IPacketResponder<RbossPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RbossResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public RbossResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RbossPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Result.FromSuccess();
        }

        var bossEntity = packet.EntityId is not null ? map.Entities.GetEntity<Monster>(packet.EntityId.Value) : null;

        RaidState? previousState = null;
        var currentRaid = await _game.UpdateRaidAsync
        (
            raid =>
            {
                previousState = raid.State;
                if (bossEntity is not null && (raid.Bosses is null || !raid.Bosses.Contains(bossEntity)))
                {
                    return raid with
                    {
                        Boss = bossEntity,
                        Bosses = (raid.Bosses ?? Array.Empty<Monster>()).Append(bossEntity).ToList(),
                        State = RaidState.BossFight
                    };
                }

                return raid with
                { // this will oscillate between more bosses ...
                    Boss = bossEntity
                };
            },
            ct: ct
        );

        if (currentRaid is not null && previousState is not null && previousState != currentRaid.State)
        {
            return await _eventDispatcher.DispatchEvent
                (new RaidStateChangedEvent(previousState.Value, currentRaid.State, currentRaid), ct);
        }

        return Result.FromSuccess();
    }
}