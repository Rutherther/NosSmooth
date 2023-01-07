//
//  LivingEntityExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Game.Extensions;

/// <summary>
/// An extension methods for <see cref="ILivingEntity"/>.
/// </summary>
public static class LivingEntityExtensions
{
    /// <summary>
    /// Checks whether the entity is alive.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>Whether the entity is alive.</returns>
    public static bool IsAlive(ILivingEntity entity)
    {
        return entity.Hp is null || entity.Hp.Amount != 0 || entity.Hp.Percentage != 0;
    }
}