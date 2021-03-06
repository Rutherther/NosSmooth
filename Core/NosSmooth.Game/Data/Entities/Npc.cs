//
//  Npc.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Represents nostale npc entity.
/// </summary>
public class Npc : ILivingEntity
{
    /// <summary>
    /// Gets the VNum of the npc.
    /// </summary>
    public int VNum { get; internal set; }

    /// <inheritdoc/>
    public long Id { get; set; }

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc />
    public bool IsSitting { get; set; }

    /// <inheritdoc />
    public bool CantMove { get; set; }

    /// <inheritdoc />
    public bool CantAttack { get; set; }

    /// <inheritdoc/>
    public Position? Position { get; set; }

    /// <inheritdoc/>
    public EntityType Type => EntityType.Npc;

    /// <inheritdoc/>
    public int? Speed { get; set; }

    /// <inheritdoc />
    public bool? IsInvisible { get; set; }

    /// <inheritdoc/>
    public ushort? Level { get; set; }

    /// <inheritdoc/>
    public byte? Direction { get; set; }

    /// <inheritdoc/>
    public Health? Hp { get; set; }

    /// <inheritdoc/>
    public Health? Mp { get; set; }

    /// <inheritdoc/>
    public FactionType? Faction { get; set; }

    /// <inheritdoc/>
    public short Size { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<long>? EffectsVNums { get; set; }
}