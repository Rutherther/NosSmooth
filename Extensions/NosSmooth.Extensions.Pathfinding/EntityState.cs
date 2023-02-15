//
//  EntityState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Extensions.Pathfinding;

/// <summary>
/// A state, position of an entity.
/// </summary>
internal class EntityState
{
    /// <summary>
    /// Gets id of the entity.
    /// </summary>
    internal long Id { get; init; }

    /// <summary>
    /// Gets or sets the x coordinate of the entity.
    /// </summary>
    internal short X { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate of the entity.
    /// </summary>
    internal short Y { get; set; }
}