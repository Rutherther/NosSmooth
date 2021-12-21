//
//  LivingEntity.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Enumerations;
using NosCore.Shared.Enumerations;
using NosSmooth.Game.Data;

namespace NosSmooth.Game.Entities;

/// <summary>
/// Represents any nostale living entity such as monster, npc, player.
/// </summary>
public interface ILivingEntity : IEntity
{
    /// <summary>
    /// Gets the speed of the entity. May be null if unknown.
    /// </summary>
    public byte? Speed { get; }

    /// <summary>
    /// Gets the level of the entity. May be null if unknown.
    /// </summary>
    public ushort? Level { get; }

    /// <summary>
    /// Gets the direction the entity is looking. May be null if unknown.
    /// </summary>
    public byte? Direction { get; }

    /// <summary>
    /// Gets the percentage of the health points of the entity. May be null if unknown.
    /// </summary>
    public byte? HpPercentage { get; }

    /// <summary>
    /// Gets the percentage of the mana points of the entity. May be null if unknown.
    /// </summary>
    public byte? MpPercentage { get; }

    /// <summary>
    /// Gets the faction of the entity. May be null if unknown.
    /// </summary>
    public FactionType? Faction { get; }
}