//
//  IEntity.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums.Entities;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// Base type for entities.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the id of the entity.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Gets the position of the entity.
    /// </summary>
    public Position? Position { get; set; }

    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    public EntityType Type { get; }
}