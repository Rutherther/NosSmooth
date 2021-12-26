//
//  Morph.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Info;

/// <summary>
/// Represents players morph.
/// </summary>
/// <remarks>
/// Morphs are used mainly for special cards.
/// The VNum will contain the vnum of the special card.
/// </remarks>
/// <param name="VNum">The vnum of the morph.</param>
/// <param name="Upgrade">The upgrade to show wings.</param>
/// <param name="Design">The design of the wings.</param>
/// <param name="Bonus">Unknown.</param>
/// <param name="Skin">The skin of the wings.</param>
public record Morph
(
    long VNum,
    byte Upgrade,
    short? Design = default,
    byte? Bonus = default,
    short? Skin = default
);