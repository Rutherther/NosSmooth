//
//  IEntity.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Shared.Enumerations;
using NosSmooth.Game.Data.Info;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Base type for entities.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the id of the entity.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Gets the name of the entity. May be null if unknown.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets the position of the entity.
    /// </summary>
    public Position? Position { get; }

    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    public VisualType Type { get; }
}