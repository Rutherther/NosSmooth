//
//  FactionType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums;

/// <summary>
/// Faction of an entity.
/// </summary>
public enum FactionType
{
    /// <summary>
    /// No faction.
    /// </summary>
    Neutral = 0,

    /// <summary>
    /// Angel faction.
    /// </summary>
    Angel = 1,

    /// <summary>
    /// Demon faction.
    /// </summary>
    Demon = 2
}