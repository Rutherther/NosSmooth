//
//  LivingEntity.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Represents any nostale living entity such as monster, npc, player.
/// </summary>
public interface ILivingEntity : IEntity
{
    /// <summary>
    /// Gets the speed of the entity. May be null if unknown.
    /// </summary>
    public int? Speed { get; set; }

    /// <summary>
    /// Gets or sets whether the player is invisible.
    /// </summary>
    public bool? IsInvisible { get; set; }

    /// <summary>
    /// Gets the level of the entity. May be null if unknown.
    /// </summary>
    public ushort? Level { get; set; }

    /// <summary>
    /// Gets the direction the entity is looking. May be null if unknown.
    /// </summary>
    public byte? Direction { get; set; }

    /// <summary>
    /// Gets the percentage of the health points of the entity. May be null if unknown.
    /// </summary>
    public Health? Hp { get; set; }

    /// <summary>
    /// Gets the percentage of the mana points of the entity. May be null if unknown.
    /// </summary>
    public Health? Mp { get; set; }

    /// <summary>
    /// Gets the faction of the entity. May be null if unknown.
    /// </summary>
    public FactionType? Faction { get; set; }

    /// <summary>
    /// Gets the size of the entity.
    /// </summary>
    public short Size { get; set; }

    /// <summary>
    /// Gets the VNums of the effects the entity has.
    /// </summary>
    public IReadOnlyList<long>? EffectsVNums { get; set; }

    /// <summary>
    /// Gets the name of the entity. May be null if unknown.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets whether the entity is sitting.
    /// </summary>
    public bool IsSitting { get; set; }

    /// <summary>
    /// Gets or sets whether the entity cannot move.
    /// </summary>
    public bool CantMove { get; set; }

    /// <summary>
    /// Gets or sets whether the entity cannot attack.
    /// </summary>
    public bool CantAttack { get; set; }
}