//
//  PlayerSkillResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Skills;

/// <summary>
/// Responds to SkiPacket to add skill to the character.
/// </summary>
public class PlayerSkillResponder : IPacketResponder<SkiPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<PlayerSkillResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerSkillResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public PlayerSkillResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<PlayerSkillResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<SkiPacket> packetArgs, CancellationToken ct = default)
    {
        // TODO: put all code into CreateOrUpdate to avoid concurrent problems.
        var packet = packetArgs.Packet;

        Skill primarySkill, secondarySkill;

        var skills = _game.Skills;

        if (packet.PrimarySkillVNum == skills?.PrimarySkill.SkillVNum)
        {
            primarySkill = skills.PrimarySkill;
        }
        else
        {
            primarySkill = await CreateSkill(packet.PrimarySkillVNum, default);
        }

        if (packet.PrimarySkillVNum == packet.SecondarySkillVNum)
        {
            secondarySkill = primarySkill;
        }
        else if (packet.SecondarySkillVNum == skills?.SecondarySkill.SkillVNum)
        {
            secondarySkill = skills.SecondarySkill;
        }
        else
        {
            secondarySkill = await CreateSkill(packet.SecondarySkillVNum, default);
        }

        var skillsFromPacket = packet.SkillSubPackets?.Select(x => x.SkillVNum).ToList() ?? new List<int>();
        var skillsFromCharacter = skills is null
            ? new List<int>()
            : skills.OtherSkills.Select(x => x.SkillVNum).ToList();
        var newSkills = skillsFromPacket.Except(skillsFromCharacter);
        var oldSkills = skillsFromCharacter.Except(skillsFromPacket);

        var otherSkillsFromCharacter = new List<Skill>(skills?.OtherSkills ?? new Skill[] { });
        otherSkillsFromCharacter.RemoveAll(x => oldSkills.Contains(x.SkillVNum));

        foreach (var newSkill in newSkills)
        {
            otherSkillsFromCharacter.Add(await CreateSkill(newSkill, default));
        }

        otherSkillsFromCharacter.RemoveAll(x => x.SkillVNum == primarySkill.SkillVNum || x.SkillVNum == secondarySkill.SkillVNum);
        otherSkillsFromCharacter.RemoveAll(x => x.Info?.SkillType != SkillType.Player);
        skills = new Data.Characters.Skills(primarySkill, secondarySkill, otherSkillsFromCharacter);

        await _game.CreateOrUpdateSkillsAsync
        (
            () => skills,
            _ => skills,
            ct: ct
        );

        await _eventDispatcher.DispatchEvent(new SkillsReceivedEvent(skills), ct);

        return Result.FromSuccess();
    }

    private async Task<Skill> CreateSkill(int vnum, int? level)
    {
        var infoResult = await _infoService.GetSkillInfoAsync(vnum);
        if (!infoResult.IsSuccess)
        {
            _logger.LogWarning("Could not obtain a skill info for vnum {vnum}: {error}", vnum, infoResult.ToFullString());
        }

        return new Skill(vnum, level, infoResult.IsSuccess ? infoResult.Entity : null);
    }
}