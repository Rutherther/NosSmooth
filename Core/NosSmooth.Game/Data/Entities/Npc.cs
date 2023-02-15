//
//  Npc.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums.Entities;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Represents nostale npc entity.
/// </summary>
public class Npc : ILivingEntity
{
    /// <summary>
    /// Gets or sets the monster info.
    /// </summary>
    public IMonsterInfo? NpcInfo { get; set; }

    /// <inheritdoc/>
    public int VNum { get; set; }

    /// <inheritdoc/>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the id of the owner, if any.
    /// </summary>
    public long? OwnerId { get; set; }

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <inheritdoc />
    public bool IsSitting { get; set; }

    /// <inheritdoc />
    public bool CantMove { get; set; }

    /// <inheritdoc />
    public bool CantAttack { get; set; }

    /// <summary>
    /// Gets or sets whether the entity is a partner.
    /// </summary>
    public bool? IsPartner { get; set; }

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
    public IReadOnlyList<short>? EffectsVNums { get; set; }

    /// <summary>
    /// Gets or sets the skills.
    /// </summary>
    public IReadOnlyList<Skill>? Skills { get; set; }
}