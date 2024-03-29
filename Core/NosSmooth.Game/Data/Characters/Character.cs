﻿//
//  Character.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Chat;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Data.Characters;

/// <summary>
/// Represents the client character.
/// </summary>
public class Character : Player
{
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

    /// <summary>
    /// Gets or sets the sp points of the player.
    /// </summary>
    /// <remarks>
    /// Resets every day, max 10 000.
    /// </remarks>
    public int SpPoints { get; set; }

    /// <summary>
    /// Gets or sets the additional sp points of the player.
    /// </summary>
    /// <remarks>
    /// Used if <see cref="SpPoints"/> are 0. Max 1 000 000.
    /// </remarks>
    public int AdditionalSpPoints { get; set; }
}