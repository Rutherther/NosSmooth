//
//  Player.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Items;
using NosSmooth.Game.Data.Social;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Players;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Represents nostale player entity.
/// </summary>
public class Player : ILivingEntity
{
    /// <summary>
    /// Gets or sets the authority of the player.
    /// </summary>
    public AuthorityType Authority { get; set; }

    /// <summary>
    /// Gets or sets the sex of the player.
    /// </summary>
    public SexType Sex { get; set; }

    /// <summary>
    /// Gets or sets the hairstyle of the player.
    /// </summary>
    public HairStyle HairStyle { get; set; }

    /// <summary>
    /// Gets or sets the hair color of the player.
    /// </summary>
    public HairColor HairColor { get; set; }

    /// <summary>
    /// Gets or sets the class of the player.
    /// </summary>
    public PlayerClass Class { get; set; }

    /// <summary>
    /// Gets or sets the reputation icon. UNKNOWN TODO.
    /// </summary>
    public short? Icon { get; set; }

    /// <summary>
    /// UNKNOWN TODO.
    /// </summary>
    public short? Compliment { get; set; }

    /// <summary>
    /// Gets or sets the morph used for sps, vehicles and such.
    /// </summary>
    public Morph? Morph { get; set; }

    /// <summary>
    /// Gets or sets whether the player is a champion arena winner.
    /// </summary>
    public bool ArenaWinner { get; set; }

    /// <summary>
    /// Gets or sets the reputation number of the player.
    /// </summary>
    public long? Reputation { get; set; }

    /// <summary>
    /// Gets or sets the visible title of the player.
    /// </summary>
    public short Title { get; set; }

    /// <summary>
    /// Gets or sets the family.
    /// </summary>
    public Family? Family { get; set; }

    /// <summary>
    /// Gets the VNum of the npc.
    /// </summary>
    public int VNum { get; set; }

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
    public EntityType Type => EntityType.Player;

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

    /// <summary>
    /// Gets or sets the hero level.
    /// </summary>
    public virtual short? HeroLevel { get; set; }

    /// <summary>
    /// Gets or sets the equipment.
    /// </summary>
    public Equipment? Equipment { get; set; }
}