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
public class Character : Player
{
    /// <summary>
    /// Gets or sets whether the character can't move.
    /// </summary>
    public bool Stunned { get; set; }

    /// <summary>
    /// Gets or sets the inventory of the character.
    /// </summary>
    public Inventory.Inventory? Inventory { get; set; }

    /// <summary>
    /// Get or sets the friends of the character.
    /// </summary>
    public IReadOnlyList<Friend>? Friends { get; set; }

    /// <summary>
    /// Gets or sets the skills of the player.
    /// </summary>
    public Skills? Skills { get; set; }

    /// <summary>
    /// Gets or sets the group the player is in.
    /// </summary>
    public Group? Group { get; set; }

    /// <summary>
    /// Gets or sets the c skill points.
    /// </summary>
    public int? SkillCp { get; set; }

    /// <summary>
    /// Gets or sets the job level.
    /// </summary>
    public Level? JobLevel { get; set; }

    /// <summary>
    /// Gets or sets the player level.
    /// </summary>
    public Level? PlayerLevel { get; set; }

    /// <summary>
    /// Gets or sets the player level.
    /// </summary>
    public Level? HeroLevelStruct { get; set; }

    /// <inheritdoc/>
    public override short? HeroLevel
    {
        get => HeroLevelStruct?.Lvl;
        set
        {
            if (HeroLevelStruct is not null && value is not null)
            {
                HeroLevelStruct = HeroLevelStruct with
                {
                    Lvl = value.Value
                };
            }
        }
    }
}