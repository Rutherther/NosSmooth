//
//  PathfinderState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using NosSmooth.Core.Client;
using NosSmooth.Core.Stateful;
using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Extensions.Pathfinding;

/// <summary>
/// State of the <see cref="Pathfinder"/>.
/// </summary>
public class PathfinderState : IStatefulEntity
{
    private ConcurrentDictionary<long, EntityState> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="PathfinderState"/> class.
    /// </summary>
    public PathfinderState()
    {
        _entities = new ConcurrentDictionary<long, EntityState>();
        Character = new EntityState();
    }

    /// <summary>
    /// Gets the entities.
    /// </summary>
    internal IReadOnlyDictionary<long, EntityState> Entities => _entities;

    /// <summary>
    /// Gets the character entity state.
    /// </summary>
    internal EntityState Character { get; private set; }

    /// <summary>
    /// Gets or sets the current map id.
    /// </summary>
    internal int? MapId { get; set; }

    /// <summary>
    /// Gets or sets the current map information.
    /// </summary>
    internal IMapInfo? MapInfo { get; set; }

    /// <summary>
    /// Sets the id of the character entity.
    /// </summary>
    /// <param name="characterId">The character id.</param>
    internal void SetCharacterId(long characterId)
    {
        EntityState GetCharacter()
        {
            Character = new EntityState
            {
                Id = characterId,
                X = Character.X,
                Y = Character.Y
            };

            return Character;
        }

        _entities.TryRemove(Character.Id, out _);
        _entities.AddOrUpdate
        (
            characterId,
            _ => GetCharacter(),
            (_, _) => GetCharacter()
        );
    }

    /// <summary>
    /// Add the given entity to the list.
    /// </summary>
    /// <param name="entityId">The id of the entity.</param>
    /// <param name="x">The x coordinate of the entity.</param>
    /// <param name="y">The y coordinate of the entity.</param>
    internal void AddEntity
    (
        long entityId,
        short x,
        short y
    )
    {
        EntityState GetEntity()
        {
            return new EntityState
            {
                Id = entityId,
                X = x,
                Y = y
            };
        }

        _entities.AddOrUpdate(entityId, _ => GetEntity(), (_, _) => GetEntity());
    }

    /// <summary>
    /// Remove all entities from the list.
    /// </summary>
    internal void ClearEntities()
    {
        _entities.Clear();
        SetCharacterId(Character.Id);
    }
}