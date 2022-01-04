//
//  Player.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Players;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Represents nostale player entity.
/// </summary>
/// <param name="Id">The id of the player.</param>
/// <param name="Name">The name of the player.</param>
/// <param name="Position">The position the player is at.</param>
/// <param name="Speed"></param>
/// <param name="Level"></param>
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
public record Player
(
    long Id,
    string? Name = default,
    Position? Position = default,
    byte? Speed = default,
    Level? Level = default,
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
) : ILivingEntity
{
    /// <inheritdoc/>
    ushort? ILivingEntity.Level => Level?.Lvl;

    /// <inheritdoc />
    public EntityType Type => EntityType.Player;
}