//
//  GroupMember.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums.Players;

namespace NosSmooth.Game.Data.Social;

/// <summary>
/// A member of a group the character is in.
/// </summary>
/// <param name="PlayerId">The id of the group member player.</param>
public record GroupMember(long PlayerId)
{
    /// <summary>
    /// Gets the level of the member.
    /// </summary>
    public byte Level { get; internal set; }

    /// <summary>
    /// Gets the hero level of the member.
    /// </summary>
    public byte? HeroLevel { get; internal set; }

    /// <summary>
    /// Gets the name of the member.
    /// </summary>
    public string? Name { get; internal set; }

    /// <summary>
    /// Gets the class of the member.
    /// </summary>
    public PlayerClass Class { get; internal set; }

    /// <summary>
    /// Gets the sex of the member.
    /// </summary>
    public SexType Sex { get; internal set; }

    /// <summary>
    /// Gets the morph vnum of the player.
    /// </summary>
    public int? MorphVNum { get; internal set; }

    /// <summary>
    /// Gets the hp of the member.
    /// </summary>
    public Health? Hp { get; internal set; }

    /// <summary>
    /// Gets the mp of the member.
    /// </summary>
    public Health? Mp { get; internal set; }

    /// <summary>
    /// Gets the effects of the member.
    /// </summary>
    public IReadOnlyList<short>? EffectsVNums { get; internal set; }
}