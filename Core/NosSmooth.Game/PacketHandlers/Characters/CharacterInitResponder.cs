//
//  CharacterInitResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Packets.Packets.Server.Players;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Characters;

/// <summary>
/// Responds to CInfoPacket by creating the character.
/// </summary>
public class CharacterInitResponder : IPacketResponder<CInfoPacket>, IPacketResponder<LevPacket>, IPacketResponder<CModePacket>
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
        var character = oldCharacter;
        var packet = packetArgs.Packet;
        await _game.EnsureCharacterCreatedAsync(false, ct).ConfigureAwait(false);
        if (character is not null)
        {
            _game.Character = character = character with
            {
                Id = packet.CharacterId,
                AuthorityType = packet.Authority,
                Sex = packet.Sex,
                HairStyle = packet.HairStyle,
                HairColor = packet.HairColor,
                Class = packet.Class,
                Icon = packet.Icon,
                Compliment = packet.Compliment,
                Morph = (character.Morph ?? new Morph(packet.MorphVNum, packet.MorphUpgrade)) with
                {
                    VNum = packet.MorphVNum, Upgrade = packet.MorphUpgrade
                },
                ArenaWinner = packet.ArenaWinner,
                Invisible = packet.IsInvisible,
                Family = new Family(packet.FamilyId, packet.FamilyName, packet.FamilyLevel)
            };
        }

        _game.SetSemaphore.Release();
        if (character is not null && character != oldCharacter)
        {
            return await _eventDispatcher.DispatchEvent(new ReceivedCharacterDataEvent(oldCharacter, character), ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<LevPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        await _game.EnsureCharacterCreatedAsync(false, ct).ConfigureAwait(false);
        var oldCharacter = _game.Character;
        var character = oldCharacter;

        if (character is not null)
        {
            _game.Character = character = character with
            {
                SkillCp = packet.SkillCp,
                Reputation = packet.Reputation,
                Level = new Level(packet.Level, packet.LevelXp, packet.XpLoad),
                JobLevel = new Level(packet.JobLevel, packet.JobLevelXp, packet.JobXpLoad),
                HeroLevel = new Level(packet.HeroLevel, packet.HeroXp, packet.HeroXpLoad)
            };
        }

        _game.SetSemaphore.Release();
        if (character is not null && character != oldCharacter)
        {
            return await _eventDispatcher.DispatchEvent(new ReceivedCharacterDataEvent(oldCharacter, character), ct);
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CModePacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var oldCharacter = _game.Character;
        var character = oldCharacter;

        if (character is null || character.Id != packetArgs.Packet.EntityId)
        { // Not the current character.
            return Result.FromSuccess();
        }

        await _game.SetSemaphore.WaitAsync(ct);
        character = character with
        {
            Morph = new Morph
            (
                packet.MorphVNum,
                packet.MorphUpgrade,
                packet.MorphDesign,
                packet.MorphBonus,
                packet.MorphSkin
            ),
            Size = packet.Size
        };

        _game.SetSemaphore.Release();
        if (oldCharacter != character)
        {
            return await _eventDispatcher.DispatchEvent(new ReceivedCharacterDataEvent(oldCharacter, character), ct);
        }

        return Result.FromSuccess();
    }
}