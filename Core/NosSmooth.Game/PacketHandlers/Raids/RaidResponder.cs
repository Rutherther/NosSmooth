//
//  RaidResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http.Headers;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Raids;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Raids;
using NosSmooth.Packets.Enums.Raids;
using NosSmooth.Packets.Server.Raids;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Raids;

/// <summary>
/// A responder to <see cref="RaidPacket"/>.
/// </summary>
public class RaidResponder : IPacketResponder<RaidPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public RaidResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;

    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RaidPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.Type is not(RaidPacketType.Leader or RaidPacketType.ListMembers or RaidPacketType.PlayerHealths or RaidPacketType.Leave))
        {
            return Result.FromSuccess();
        }

        Raid? prevRaid = null;
        var currentRaid = await _game.UpdateRaidAsync
        (
            raid =>
            {
                prevRaid = raid;
                switch (packet.Type)
                {
                    case RaidPacketType.Leave:
                        if (packet.LeaveType is not null && packet.LeaveType == RaidLeaveType.PlayerLeft)
                        { // the player has left.
                            prevRaid = raid with
                            {
                                State = RaidState.Left
                            };

                            return null;
                        }

                        return raid;
                    case RaidPacketType.Leader:
                        if (packet.LeaderId is null)
                        { // set the raid to null.
                            return null;
                        }

                        return raid with
                        {
                            Leader = raid.Members?.FirstOrDefault(x => x.PlayerId == packet.LeaderId.Value)
                        };
                    case RaidPacketType.ListMembers:
                        return raid with
                        {
                            Members = raid.Members?.Where(x => packet.ListMembersPlayerIds?.Contains(x.PlayerId) ?? true).ToList()
                        };
                    case RaidPacketType.PlayerHealths:
                        // update healths
                        foreach (var member in raid.Members ?? (IReadOnlyList<GroupMember>)Array.Empty<GroupMember>())
                        {
                            var data = packet.PlayerHealths?.FirstOrDefault(x => x.PlayerId == member.PlayerId);

                            if (data is not null)
                            {
                                member.Hp ??= new Health();
                                member.Mp ??= new Health();

                                member.Hp.Percentage = data.HpPercentage;
                                member.Mp.Percentage = data.MpPercentage;
                            }
                        }
                        return raid;
                }

                return raid;
            },
            ct: ct
        );

        if (currentRaid is null && prevRaid is not null)
        {
            return await _eventDispatcher.DispatchEvent(new RaidFinishedEvent(prevRaid), ct);
        }

        return Result.FromSuccess();
    }
}