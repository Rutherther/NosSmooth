//
//  ISkillInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Abstractions.Infos;

/// <summary>
/// The NosTale skill information.
/// </summary>
public interface ISkillInfo : IVNumInfo
{
    /// <summary>
    /// Gets the translatable name of the skill.
    /// </summary>
    TranslatableString Name { get; }

    /// <summary>
    /// Gets the tile range of the skill.
    /// </summary>
    short Range { get; }

    /// <summary>
    /// Gets the zone tiles range.
    /// </summary>
    short ZoneRange { get; }

    /// <summary>
    /// Gets the time it takes to cast this skill. The unit is Tenth of a second. (10 means one second).
    /// </summary>
    int CastTime { get; }

    /// <summary>
    /// Gets the time of the cooldown. The unit is Tenth of a second. (10 means one second).
    /// </summary>
    int Cooldown { get; }

    /// <summary>
    /// Gets the type of the skill.
    /// </summary>
    SkillType SkillType { get; }

    /// <summary>
    /// Gets the attack type of the skill.
    /// </summary>
    AttackType AttackType { get; }

    /// <summary>
    /// Gets whether the skill uses secondary weapon, primary weapon is used if false.
    /// </summary>
    bool UsesSecondaryWeapon { get; }

    /// <summary>
    /// Gets the mana points the skill cast costs.
    /// </summary>
    int MpCost { get; }

    /// <summary>
    /// Gets the cast id of the skill used in u_s, su packets.
    /// </summary>
    short CastId { get; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    TargetType TargetType { get; }

    /// <summary>
    /// Gets the hit type of the skill.
    /// </summary>
    HitType HitType { get; }

    /// <summary>
    /// Gets the element of the skill.
    /// </summary>
    Element Element { get; }

    /// <summary>
    /// Gets the cost of the skill.
    /// </summary>
    int SpecialCost { get; }

    /// <summary>
    /// Gets the upgrade skill.
    /// </summary>
    short Upgrade { get; }

    /// <summary>
    /// Gets the upgrade type (morph).
    /// </summary>
    short MorphOrUpgrade { get; }

    /// <summary>
    /// Gets the speed of the dash.
    /// </summary>
    short DashSpeed { get; }

    /// <summary>
    /// Gets vnum of an item that is consumed by the skill.
    /// </summary>
    int ItemVNum { get; }
}