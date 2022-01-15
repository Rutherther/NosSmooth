//
//  SkillResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Characters;

/// <summary>
/// Responds to SkiPacket to add skill to the character.
/// </summary>
public class SkillResponder : IPacketResponder<SkiPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public SkillResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<SkiPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        Skill primarySkill, secondarySkill;

        var character = _game.Character;

        if (character is not null && packet.PrimarySkillId == character.Skills?.PrimarySkill.SkillVNum)
        {
            primarySkill = character.Skills.PrimarySkill;
        }
        else
        {
            primarySkill = new Skill(packet.PrimarySkillId);
        }

        if (character is not null && packet.PrimarySkillId == packet.SecondarySkillId)
        {
            secondarySkill = primarySkill;
        }
        else if (character is not null && packet.SecondarySkillId == character.Skills?.SecondarySkill.SkillVNum)
        {
            secondarySkill = character.Skills.SecondarySkill;
        }
        else
        {
            secondarySkill = new Skill(packet.SecondarySkillId);
        }

        var skillsFromPacket = packet.SkillSubPackets?.Select(x => x.SkillId).ToList() ?? new List<long>();
        var skillsFromCharacter = character?.Skills is null
            ? new List<long>()
            : character.Skills.OtherSkills.Select(x => x.SkillVNum).ToList();
        var newSkills = skillsFromPacket.Except(skillsFromCharacter);
        var oldSkills = skillsFromCharacter.Except(skillsFromPacket);

        var otherSkillsFromCharacter = new List<Skill>(character?.Skills?.OtherSkills ?? new Skill[] { });
        otherSkillsFromCharacter.RemoveAll(x => oldSkills.Contains(x.SkillVNum));

        foreach (var newSkill in newSkills)
        {
            otherSkillsFromCharacter.Add(new Skill(newSkill));
        }

        var skills = new Skills(primarySkill, secondarySkill, otherSkillsFromCharacter);

        await _game.CreateOrUpdateCharacterAsync
        (
            () => new Character(Skills: skills),
            c => c with { Skills = skills },
            ct: ct
        );

        await _eventDispatcher.DispatchEvent(new SkillsReceivedEvent(skills), ct);

        return Result.FromSuccess();
    }
}