//
//  AttackType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// A type of skill attack.
/// </summary>
public enum AttackType
{
    /// <summary>
    /// A melee attack.
    /// </summary>
    Melee = 0,

    /// <summary>
    /// A ranged attack.
    /// </summary>
    Ranged = 1,

    /// <summary>
    /// A magical attack.
    /// </summary>
    Magical = 2,

    /// <summary>
    /// Another type of attack.
    /// </summary>
    /// <remarks>
    /// Used only for 3 skills.
    /// 1. Pajama's (sp) pillow fight.
    /// 2. Chicken's (sp) jump.
    /// 3. Chicken's (sp) kick.
    /// </remarks>
    Other = 3,

    /// <summary>
    /// Charged attacks.
    /// </summary>
    /// <remarks>
    /// Seems no different from melee or ranged.
    /// </remarks>
    Charge = 4,

    /// <summary>
    /// A dash attack.
    /// </summary>
    /// <remarks>
    /// Should send location to dash to to the server.
    /// </remarks>
    Dash = 5
}