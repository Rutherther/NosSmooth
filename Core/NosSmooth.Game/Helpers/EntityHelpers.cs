//
//  EntityHelpers.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Helpers;

/// <summary>
/// Helper methods for various operations with entities.
/// </summary>
public static class EntityHelpers
{
    /// <summary>
    /// Create an entity from the given type and id.
    /// </summary>
    /// <param name="type">The entity type.</param>
    /// <param name="entityId">The entity id.</param>
    /// <returns>The entity.</returns>
    public static IEntity CreateEntity(EntityType type, long entityId)
    {
        switch (type)
        {
            case EntityType.Npc:
                return new Npc
                {
                    Id = entityId
                };
            case EntityType.Monster:
                return new Monster
                {
                    Id = entityId
                };
            case EntityType.Player:
                return new Player
                {
                    Id = entityId
                };
            case EntityType.Object:
                return new GroundItem
                {
                    Id = entityId
                };
        }

        throw new Exception("Unknown entity type.");
    }
}