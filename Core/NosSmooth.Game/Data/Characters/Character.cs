//
//  Character.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Chat;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Players;

namespace NosSmooth.Game.Data.Characters;

/// <summary>
/// Represents the client character.
/// </summary>
/// <param name="Inventory">The character's inventory with items.</param>
/// <param name="Family">The family of the character, if any..</param>
/// <param name="Friends">The friends of the character.</param>
/// <param name="Skills">The current skill set of the character.</param>
/// <param name="Group">The group the character is in, if any. Contains pets and partners as well.</param>
/// <param name="SkillCp">The skill cp amount used for learning new skills.</param>
/// <param name="Id">The id of the character entity.</param>
/// <param name="Name">The name of the character entity.</param>
/// <param name="Position">The position of the character.</param>
/// <param name="Speed">The movement speed of the character.</param>
/// <param name="Level">The </param>
/// <param name="JobLevel">The </param>
/// <param name="HeroLevel">The </param>
/// <param name="Direction"></param>
/// <param name="Hp"></param>
/// <param name="Mp"></param>
/// <param name="Faction"></param>
/// <param name="Size"></param>
/// <param name="AuthorityType"></param>
/// <param name="Sex"></param>
/// <param name="HairStyle"></param>
/// <param name="HairColor"></param>
/// <param name="Class"></param>
/// <param name="Icon"></param>
/// <param name="Compliment"></param>
/// <param name="Morph"></param>
/// <param name="ArenaWinner"></param>
/// <param name="Invisible"></param>
/// <param name="Reputation"></param>
public record Character
(
    Inventory.Inventory? Inventory = default,
    Family? Family = default,
    IReadOnlyList<Friend>? Friends = default,
    Skills? Skills = default,
    Group? Group = default,
    int? SkillCp = default,
    long Id = default,
    string? Name = default,
    Position? Position = default,
    byte? Speed = default,
    Level? Level = default,
    Level? JobLevel = default,
    Level? HeroLevel = default,
    byte? Direction = default,
    Health? Hp = default,
    Health? Mp = default,
    FactionType? Faction = default,
    short Size = default,
    AuthorityType AuthorityType = default,
    SexType Sex = default,
    HairStyle HairStyle = default,
    HairColor HairColor = default,
    PlayerClass Class = default,
    byte? Icon = default,
    short? Compliment = default,
    Morph? Morph = default,
    bool? ArenaWinner = default,
    bool? Invisible = default,
    long? Reputation = default,
    IReadOnlyList<long>? EffectsVNums = default
) : Player(
    Id,
    Name,
    Position,
    Speed,
    Level,
    HeroLevel,
    Direction,
    Hp,
    Mp,
    Faction,
    Size,
    AuthorityType,
    Sex,
    HairStyle,
    HairColor,
    Class,
    Icon,
    Compliment,
    Morph,
    ArenaWinner,
    Invisible,
    Reputation,
    EffectsVNums
);