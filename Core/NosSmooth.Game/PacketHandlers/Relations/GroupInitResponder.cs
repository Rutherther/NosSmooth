//
//  GroupInitResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Linq.Expressions;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Groups;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Groups;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Relations;

/// <summary>
/// A group initialization responder.
/// </summary>
public class GroupInitResponder : IPacketResponder<PinitPacket>, IPacketResponder<PstPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupInitResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public GroupInitResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PinitPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        Group BuildGroup(Group? group)
        {
            var members = packet.PinitSubPackets?
                .Where(x => x.PlayerSubPacket is not null)
                .OrderBy(x => x.PlayerSubPacket!.GroupPosition)
                .Select(e => CreateEntity(e, group?.Members))
                .ToList() ?? new List<GroupMember>();

            return new Group(null, members);
        }

        var group = await _game.CreateOrUpdateGroupAsync
        (
            () => BuildGroup(null),
            BuildGroup,
            ct: ct
        );

        if (group is null)
        {
            throw new UnreachableException();
        }

        return await _eventDispatcher.DispatchEvent(new GroupInitializedEvent(group), ct);
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PstPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.EntityType != EntityType.Player)
        {
            return Result.FromSuccess();
        }

        GroupMember? member = null;
        await _game.CreateOrUpdateGroupAsync
        (
            () => null,
            g =>
            {
                member = g.Members?.FirstOrDefault(x => x.PlayerId == packet.EntityId);

                if (member is not null)
                {
                    member.Hp = new Health { Amount = packet.Hp, Percentage = packet.HpPercentage };
                    member.Mp = new Health { Amount = packet.Mp, Percentage = packet.MpPercentage };
                    member.Class = packet.PlayerClass ?? member.Class;
                    member.Sex = packet.PlayerSex ?? member.Sex;
                    member.EffectsVNums = packet.Effects?.Select(x => x.CardId).ToList();
                    member.MorphVNum = packet.PlayerMorphVNum ?? member.MorphVNum;
                }

                return g;
            },
            ct: ct
        );

        if (member is not null)
        {
            await _eventDispatcher.DispatchEvent
            (
                new GroupMemberStatEvent(member),
                ct
            );
        }

        return Result.FromSuccess();
    }

    private GroupMember CreateEntity(PinitSubPacket packet, IReadOnlyList<GroupMember>? members)
    {
        var playerSubPacket = packet.PlayerSubPacket!;
        var originalMember = members?.FirstOrDefault(x => x.PlayerId == packet.EntityId);

        return new(packet.EntityId)
        {
            Level = playerSubPacket.Level,
            HeroLevel = playerSubPacket.HeroLevel,
            Name = playerSubPacket.Name?.Name,
            Class = playerSubPacket.Class,
            Sex = playerSubPacket.Sex,
            MorphVNum = playerSubPacket.MorphVNum,
            Hp = originalMember?.Hp,
            Mp = originalMember?.Mp,
            EffectsVNums = originalMember?.EffectsVNums
        };
    }
}