//
//  MatesSkillResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Mates;
using NosSmooth.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Skills;

/// <summary>
/// Responds to petski and pski packets.
/// </summary>
public class MatesSkillResponder : IPacketResponder<PetskiPacket>, IPacketResponder<PSkiPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<MatesSkillResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatesSkillResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public MatesSkillResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<MatesSkillResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PetskiPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        Skill? skill = null;
        if (packet.SkillVNum is not null)
        {
            var skillInfoResult = await _infoService.GetSkillInfoAsync(packet.SkillVNum.Value, ct);
            if (!skillInfoResult.IsDefined(out var skillInfo))
            {
                _logger.LogWarning
                (
                    "Could not obtain skill info for vnum {vnum}: {error}",
                    packet.SkillVNum.Value,
                    skillInfoResult.ToFullString()
                );
            }

            skill = new Skill(packet.SkillVNum.Value, null, skillInfo);
        }

        var mates = await _game.CreateOrUpdateMatesAsync
        (
            () => new Mates
            {
                PetSkill = skill
            },
            m =>
            {
                m.PetSkill = skill;
                return m;
            },
            ct: ct
        );

        return await _eventDispatcher.DispatchEvent
        (
            new PetSkillReceivedEvent(mates?.CurrentPet?.Pet, skill),
            ct
        );
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<PSkiPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var skills = new List<Skill>();

        foreach (var skillVNum in packet.SkillVNums)
        {
            if (skillVNum is null)
            {
                continue;
            }

            var skillInfoResult = await _infoService.GetSkillInfoAsync(skillVNum.Value, ct);
            if (!skillInfoResult.IsDefined(out var skillInfo))
            {
                _logger.LogWarning
                (
                    "Could not obtain skill info for vnum {vnum}: {error}",
                    skillVNum,
                    skillInfoResult.ToFullString()
                );
            }

            skills.Add(new Skill(skillVNum.Value, null, skillInfo));
        }

        if (skills.Count == 0)
        {
            skills = null;
        }

        var mates = await _game.CreateOrUpdateMatesAsync
        (
            () => new Mates
            {
                PartnerSkills = skills
            },
            m =>
            {
                m.PartnerSkills = skills;
                return m;
            },
            ct: ct
        );

        return await _eventDispatcher.DispatchEvent
        (
            new PartnerSkillsReceivedEvent(mates?.CurrentPartner?.Partner, skills ?? Array.Empty<Skill>().ToList()),
            ct
        );
    }
}