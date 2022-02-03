//
//  AoeSkillUsedResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Events.Battle;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Extensions;
using NosSmooth.Game.Helpers;
using NosSmooth.Packets.Server.Battle;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responder to bs packet.
/// </summary>
public class AoeSkillUsedResponder : IPacketResponder<BsPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly Logger<AoeSkillUsedResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AoeSkillUsedResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public AoeSkillUsedResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        Logger<AoeSkillUsedResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;

    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<BsPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var caster = _game.CurrentMap?.Entities.GetEntity<ILivingEntity>
            (packet.CasterEntityId) ?? (ILivingEntity)EntityHelpers.CreateEntity
            (packet.CasterEntityType, packet.CasterEntityId);
        Skill? skillEntity = null;

        if (caster is Character character)
        {
            var skillResult = character.Skills?.TryGetSkillByVNum(packet.SkillVNum);
            if (skillResult?.IsSuccess ?? false)
            {
                skillEntity = skillResult.Value.Entity;
            }
        }

        if (skillEntity is null)
        {
            var skillInfoResult = await _infoService.GetSkillInfoAsync(packet.SkillVNum, ct);
            _logger.LogWarning
            (
                "Could not obtain a skill info for vnum {vnum}: {error}",
                packet.SkillVNum,
                skillInfoResult.ToFullString()
            );

            skillEntity = new Skill(packet.SkillVNum, Info: skillInfoResult.IsSuccess ? skillInfoResult.Entity : null);
        }

        return await _eventDispatcher.DispatchEvent
        (
            new AoESkillUsedEvent
            (
                caster,
                skillEntity,
                new Position
                (
                    packet.X,
                    packet.Y
                )
            ),
            ct
        );
    }
}