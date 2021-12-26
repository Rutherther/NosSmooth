//
//  Health.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Info;

/// <summary>
/// Represents the health or mana of an entity.
/// </summary>
/// <param name="Amount">The current amount of health.</param>
/// <param name="Maximum">The maximum amount of health.</param>
public record Health(long Amount, long Maximum)
{
    private decimal Percentage => (decimal)Amount / Maximum;
}