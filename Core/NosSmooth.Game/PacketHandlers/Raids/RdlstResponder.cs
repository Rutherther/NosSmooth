//
//  RdlstResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Raids;
using NosSmooth.Packets.Server.Raids;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Raids;

/// <summary>
/// A responder to <see cref="RdlstPacket"/>.
/// </summary>
public class RdlstResponder : IPacketResponder<RdlstPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RdlstResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public RdlstResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RdlstPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        IReadOnlyList<GroupMember> UpdateMembers(IReadOnlyList<GroupMember>? currentMembers)
        {
            return packet.Players
                .Select
                (
                    packetMember =>
                    {
                        var newMember = currentMembers?.FirstOrDefault
                            (member => packetMember.Id == member.PlayerId) ?? new GroupMember(packetMember.Id);

                        newMember.Class = packetMember.Class;
                        newMember.Level = packetMember.Level;
                        newMember.HeroLevel = packetMember.HeroLevel;
                        newMember.Sex = packetMember.Sex;
                        newMember.MorphVNum = packetMember.MorphVNum;

                        return newMember;
                    }
                ).ToArray();
        }

        Raid? prevRaid = null;
        var currentRaid = await _game.CreateOrUpdateRaidAsync
        (
            () => new Raid
            (
                packet.RaidType,
                RaidState.Waiting,
                packet.MinimumLevel,
                packet.MaximumLevel,
                null,
                null,
                null,
                null,
                UpdateMembers(null)
            ),
            raid =>
            {
                prevRaid = raid;
                return raid with
                {
                    Type = packet.RaidType,
                    MinimumLevel = packet.MinimumLevel,
                    MaximumLevel = packet.MaximumLevel,
                    Members = UpdateMembers(raid.Members),
                };
            },
            ct: ct
        );

        if (prevRaid is null && currentRaid is not null)
        {
            return await _eventDispatcher.DispatchEvent(new RaidJoinedEvent(currentRaid), ct);
        }

        return Result.FromSuccess();
    }
}