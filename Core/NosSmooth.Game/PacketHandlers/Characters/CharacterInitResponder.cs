//
//  CharacterInitResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Packets.Server.Character;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Characters;

/// <summary>
/// Responds to CInfoPacket by creating the character.
/// </summary>
public class CharacterInitResponder : IPacketResponder<CInfoPacket>, IPacketResponder<LevPacket>,
    IPacketResponder<CModePacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterInitResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public CharacterInitResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CInfoPacket> packetArgs, CancellationToken ct = default)
    {
        var oldCharacter = _game.Character;
        var packet = packetArgs.Packet;
        var character = await _game.CreateOrUpdateCharacterAsync
        (
            () => new Character
            {
                Id = packet.CharacterId,
                Name = packet.Name,
                Authority = packet.Authority,
                Sex = packet.Sex,
                HairStyle = packet.HairStyle,
                HairColor = packet.HairColor,
                Class = packet.Class,
                Icon = packet.Icon,
                Compliment = packet.Compliment,
                Morph = new Morph(packet.MorphVNum, packet.MorphUpgrade),
                IsInvisible = packet.IsInvisible,
                ArenaWinner = packet.ArenaWinner
            },
            (character) =>
            {
                character.Id = packet.CharacterId;
                character.Authority = packet.Authority;
                character.Sex = packet.Sex;
                character.HairStyle = packet.HairStyle;
                character.HairColor = packet.HairColor;
                character.Class = packet.Class;
                character.Icon = packet.Icon;
                character.Compliment = packet.Compliment;
                character.Morph = (character.Morph ?? new Morph(packet.MorphVNum, packet.MorphUpgrade)) with
                {
                    VNum = packet.MorphVNum, Upgrade = packet.MorphUpgrade
                };
                character.ArenaWinner = packet.ArenaWinner;
                character.IsInvisible = packet.IsInvisible;
                return character;
            },
            ct: ct
        );

        await _game.CreateOrUpdateGroupAsync
        (
            () => new Group(packet.GroupId, null),
            g => g with
            {
                Id = packet.GroupId
            },
            ct: ct
        );

        await _game.CreateOrUpdateFamilyAsync
        (
            () => new Family
            (
                packet.FamilySubPacket.Value?.FamilyId,
                packet.FamilySubPacket.Value?.Title,
                packet.FamilyName,
                packet.FamilyLevel,
                null
            ),
            f => f with
            {
                Id = packet.FamilySubPacket.Value?.FamilyId,
                Title = packet.FamilySubPacket.Value?.Title,
                Name = packet.FamilyName,
                Level = packet.FamilyLevel
            },
            ct: ct
        );

        if (character is null)
        {
            throw new UnreachableException();
        }

        if (character != oldCharacter)
        {
            return await _eventDispatcher.DispatchEvent(new ReceivedCharacterDataEvent(character), ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<LevPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var oldCharacter = _game.Character;

        var character = await _game.CreateOrUpdateCharacterAsync
        (
            () => new Character
            {
                SkillCp = packet.SkillCp,
                Reputation = packet.Reputation,
                PlayerLevel = new Level(packet.Level, packet.LevelXp, packet.XpLoad),
                JobLevel = new Level(packet.JobLevel, packet.JobLevelXp, packet.JobXpLoad),
                HeroLevelStruct = new Level(packet.HeroLevel, packet.HeroLevelXp, packet.HeroXpLoad)
            },
            (character) =>
            {
                character.SkillCp = packet.SkillCp;
                character.Reputation = packet.Reputation;
                character.PlayerLevel = new Level(packet.Level, packet.LevelXp, packet.XpLoad);
                character.JobLevel = new Level(packet.JobLevel, packet.JobLevelXp, packet.JobXpLoad);
                character.HeroLevelStruct = new Level(packet.HeroLevel, packet.HeroLevelXp, packet.HeroXpLoad);
                return character;
            },
            ct: ct
        );

        if (character is null)
        {
            throw new UnreachableException();
        }

        if (character != oldCharacter)
        {
            return await _eventDispatcher.DispatchEvent(new ReceivedCharacterDataEvent(character), ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CModePacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var oldCharacter = _game.Character;

        if (oldCharacter is null || oldCharacter.Id != packetArgs.Packet.EntityId)
        { // Not the current character.
            return Result.FromSuccess();
        }

        var character = await _game.CreateOrUpdateCharacterAsync
        (
            () => throw new NotImplementedException(),
            (character) =>
            {
                if (packet.MorphVNum is not null)
                {
                    character.Morph = new Morph
                    (
                        packet.MorphVNum.Value,
                        packet.MorphUpgrade ?? 0,
                        packet.MorphDesign,
                        packet.MorphBonus,
                        packet.MorphSkin
                    );
                }
                else
                {
                    character.Morph = null;
                }

                if (packet.Size is not null)
                {
                    character.Size = packet.Size.Value;
                }
                return character;
            },
            ct: ct
        );

        if (character is null)
        {
            throw new UnreachableException();
        }

        if (oldCharacter != character)
        {
            return await _eventDispatcher.DispatchEvent(new ReceivedCharacterDataEvent(character), ct);
        }

        return Result.FromSuccess();
    }
}