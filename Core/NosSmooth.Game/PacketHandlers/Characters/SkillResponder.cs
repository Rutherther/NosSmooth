//
//  SkillResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.ServerPackets.Battle;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
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

        var character = await _game.EnsureCharacterCreatedAsync(false, ct);

        if (packet.PrimarySkill == character.Skills?.PrimarySkill.SkillVNum)
        {
            primarySkill = character.Skills.PrimarySkill;
        }
        else
        {
            primarySkill = new Skill(packet.PrimarySkill);
        }

        if (packet.PrimarySkill == packet.SecondarySkill)
        {
            secondarySkill = primarySkill;
        }
        else if (packet.SecondarySkill == character.Skills?.SecondarySkill.SkillVNum)
        {
            secondarySkill = character.Skills.SecondarySkill;
        }
        else
        {
            secondarySkill = new Skill(packet.SecondarySkill);
        }

        var skillsFromPacket = packet.SkiSubPackets?.Select(x => x.SkillVNum).ToList() ?? new List<long>();
        var skillsFromCharacter = character.Skills is null ? new List<long>() : character.Skills.OtherSkills.Select(x => x.SkillVNum).ToList();
        var newSkills = skillsFromPacket.Except(skillsFromCharacter);
        var oldSkills = skillsFromCharacter.Except(skillsFromPacket);

        var otherSkillsFromCharacter = new List<Skill>(character.Skills?.OtherSkills ?? new Skill[] { });
        otherSkillsFromCharacter.RemoveAll(x => oldSkills.Contains(x.SkillVNum));

        foreach (var newSkill in newSkills)
        {
            otherSkillsFromCharacter.Add(new Skill(newSkill));
        }

        var skills = new Skills(primarySkill, secondarySkill, otherSkillsFromCharacter);

        _game.Character = character with
        {
            Skills = skills
        };

        _game.SetSemaphore.Release();
        await _eventDispatcher.DispatchEvent(new SkillsReceivedEvent(skills), ct);

        return Result.FromSuccess();
    }
}