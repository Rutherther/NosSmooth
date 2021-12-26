//
//  Monster.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Enumerations;
using NosCore.Shared.Enumerations;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Represents nostale monster entity.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Position"></param>
/// <param name="Speed"></param>
/// <param name="Level"></param>
/// <param name="Direction"></param>
/// <param name="Hp"></param>
/// <param name="Mp"></param>
/// <param name="Faction"></param>
/// <param name="Size"></param>
/// <param name="MonsterVNum"></param>
public record Monster
(
    long Id,
    string? Name,
    Position? Position,
    byte? Speed,
    ushort? Level,
    byte? Direction,
    Health? Hp,
    Health? Mp,
    FactionType? Faction,
    short Size,
    long MonsterVNum
) : ILivingEntity
{
    /// <inheritdoc/>
    public VisualType Type => VisualType.Monster;
}