//
//  SkillUsedResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Players;
using NosSmooth.Game.Extensions;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responds to skill used packet.
/// </summary>
public class SkillUsedResponder : IPacketResponder<SuPacket>, IPacketResponder<SrPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillUsedResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public SkillUsedResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<SuPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var character = _game.Character;

        if (packet.EntityType != EntityType.Player)
        {
            return Result.FromSuccess();
        }

        if (character is not null && character.Id != packet.VisualId && character.Skills is not null)
        {
            var skillResult = character.Skills.TryGetSkill(packet.SkillVnum);

            if (skillResult.IsDefined(out var skillEntity))
            {
                skillEntity.LastUseTime = DateTimeOffset.Now;
                skillEntity.Cooldown = TimeSpan.FromSeconds(packet.SkillCooldown / 10.0);
                skillEntity.IsOnCooldown = true;
            }

            await _eventDispatcher.DispatchEvent(
                new SkillUsedEvent
                (
                    character,
                    character.Id,
                    skillResult.IsSuccess ? skillEntity : null,
                    packet.SkillVnum,
                    packet.TargetId,
                    new Position { X = packet.PositionX, Y = packet.PositionY }
                ),
                ct);
        }
        else
        {
            // TODO: add entity from the map, if exists.
            await _eventDispatcher.DispatchEvent(
                new SkillUsedEvent
                (
                    null,
                    packet.VisualId,
                    null,
                    packet.SkillVnum,
                    packet.TargetId,
                    new Position
                    {
                        X = packet.PositionX, Y = packet.PositionY
                    }
                ),
                ct
            );
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<SrPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var character = _game.Character;

        if (character is not null && character.Skills is not null)
        {
            var skillResult = character.Skills.TryGetSkill(packet.SkillId);

            if (skillResult.IsDefined(out var skillEntity))
            {
                skillEntity.IsOnCooldown = false;
                await _eventDispatcher.DispatchEvent(new SkillReadyEvent(skillEntity, skillEntity.SkillVNum), ct);
            }
        }
        else
        {
            await _eventDispatcher.DispatchEvent(new SkillReadyEvent(null, packet.SkillId), ct);
        }

        return Result.FromSuccess();
    }
}