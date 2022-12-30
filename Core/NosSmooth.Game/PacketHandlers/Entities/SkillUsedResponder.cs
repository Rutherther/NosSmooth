//
//  SkillUsedResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Events.Battle;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Game.Extensions;
using NosSmooth.Game.Helpers;
using NosSmooth.Packets.Client.Battle;
using NosSmooth.Packets.Server.Battle;
using NosSmooth.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responds to skill used packet.
/// </summary>
public class SkillUsedResponder : IPacketResponder<SuPacket>, IPacketResponder<SrPacket>,
    IPacketResponder<UseSkillPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<SkillUsedResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillUsedResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public SkillUsedResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<SkillUsedResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<SuPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;

        // TODO: add to map if the entity is created?
        var caster = map?.Entities?.GetEntity<ILivingEntity>
            (packet.CasterEntityId) ?? (ILivingEntity)EntityHelpers.CreateEntity
            (packet.CasterEntityType, packet.CasterEntityId);
        var target = map?.Entities?.GetEntity<ILivingEntity>
            (packet.TargetEntityId) ?? (ILivingEntity)EntityHelpers.CreateEntity
            (packet.TargetEntityType, packet.TargetEntityId);

        if (target.Hp is null)
        {
            target.Hp = new Health
            {
                Percentage = packet.HpPercentage
            };
        }
        else
        {
            target.Hp.Percentage = packet.HpPercentage;
        }

        Skill? skillEntity;
        if (caster is Character character && character.Skills is not null)
        {
            var skillResult = character.Skills.TryGetSkillByVNum(packet.SkillVNum);

            if (skillResult.IsDefined(out skillEntity))
            {
                skillEntity.LastUseTime = DateTimeOffset.Now;
                skillEntity.IsOnCooldown = true;
            }
            else
            {
                var skillInfoResult = await _infoService.GetSkillInfoAsync(packet.SkillVNum, ct);
                skillEntity = new Skill
                    (packet.SkillVNum, null, skillInfoResult.IsSuccess ? skillInfoResult.Entity : null);
            }
        }
        else
        {
            var skillInfoResult = await _infoService.GetSkillInfoAsync(packet.SkillVNum, ct);
            if (!skillInfoResult.IsSuccess)
            {
                _logger.LogWarning
                (
                    "Could not obtain a skill info for vnum {vnum}: {error}",
                    packet.SkillVNum,
                    skillInfoResult.ToFullString()
                );
            }

            skillEntity = new Skill
                (packet.SkillVNum, null, skillInfoResult.IsSuccess ? skillInfoResult.Entity : null);
        }

        var dispatchResult = await _eventDispatcher.DispatchEvent
        (
            new SkillUsedEvent
            (
                caster,
                target,
                skillEntity,
                new Position(packet.PositionX, packet.PositionY),
                packet.HitMode,
                packet.Damage
            ),
            ct
        );

        if (!packet.TargetIsAlive)
        {
            target.Hp.Amount = 0;
            var diedResult = await _eventDispatcher.DispatchEvent(new EntityDiedEvent(target, skillEntity), ct);
            if (!diedResult.IsSuccess)
            {
                return dispatchResult.IsSuccess
                    ? diedResult
                    : new AggregateError(diedResult, dispatchResult);
            }
        }

        return dispatchResult;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<SrPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var character = _game.Character;

        if (character is not null && character.Skills is not null)
        {
            var skillResult = character.Skills.TryGetSkillByCastId(packet.SkillId);

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

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<UseSkillPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var character = _game.Character;

        if (character is not null && character.Skills is not null)
        {
            var skillResult = character.Skills.TryGetSkillByVNum(packet.SkillId);

            if (skillResult.IsDefined(out var skillEntity))
            {
                skillEntity.IsOnCooldown = true;
            }
        }

        return Result.FromSuccess();
    }
}