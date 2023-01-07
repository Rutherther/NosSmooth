//
//  PartyPet.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Data.Mates;

/// <summary>
/// Information about a pet that is currently in character's party.
/// </summary>
/// <param name="Pet">The underlying pet.</param>
public record PartyPet
(
    Pet Pet
)
{
    /// <summary>
    /// Gets the hp of the partner.
    /// </summary>
    public Health? Hp { get; internal set; }

    /// <summary>
    /// Gets the mp of the partner.
    /// </summary>
    public Health? Mp { get; internal set; }
}