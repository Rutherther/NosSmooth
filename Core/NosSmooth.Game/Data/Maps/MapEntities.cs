//
//  MapEntities.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Maps;

/// <summary>
/// Thread-safe store for the entities on the map.
/// </summary>
public class MapEntities
{
    private readonly ConcurrentDictionary<long, IEntity> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapEntities"/> class.
    /// </summary>
    public MapEntities()
    {
        _entities = new ConcurrentDictionary<long, IEntity>();
    }

    /// <summary>
    /// Gets the entities on the map.
    /// </summary>
    /// <returns>The list of the entities.</returns>
    public ICollection<IEntity> GetEntities()
        => _entities.Values;

    /// <summary>
    /// Gets the given entity by id.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <returns>The entity, or null, if not found.</returns>
    public IEntity? GetEntity(long id)
        => _entities.GetValueOrDefault(id);

    /// <summary>
    /// Get the given entity by id.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns>The entity.</returns>
    /// <exception cref="Exception">If the entity is not of the specified type.</exception>
    public TEntity? GetEntity<TEntity>(long id)
    {
        var entity = GetEntity(id);
        if (entity is null)
        {
            return default;
        }

        if (entity is TEntity tentity)
        {
            return tentity;
        }

        throw new Exception($"Could not find the entity with the given type {typeof(TEntity)}, was {entity.GetType()}");
    }

    /// <summary>
    /// Add the given entity to the entities list.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    internal void AddEntity(IEntity entity)
    {
        _entities.AddOrUpdate(entity.Id, _ => entity, (i, e) => entity);
    }

    /// <summary>
    /// .
    /// </summary>
    /// <param name="entityId">The id of the entity.</param>
    /// <param name="createAction">The action to execute on create.</param>
    /// <param name="updateAction">The action to execute on update.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    internal void AddOrUpdateEntity<TEntity>
        (long entityId, Func<long, TEntity> createAction, Func<long, TEntity, TEntity> updateAction)
        where TEntity : IEntity
    {
        _entities.AddOrUpdate
            (entityId, (key) => createAction(key), (key, entity) => updateAction(key, (TEntity)entity));
    }

    /// <summary>
    /// Remove the given entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    internal void RemoveEntity(IEntity entity)
    {
        RemoveEntity(entity.Id);
    }

    /// <summary>
    /// Remove the given entity.
    /// </summary>
    /// <param name="entityId">The id of the entity to remove.</param>
    internal void RemoveEntity(long entityId)
    {
        _entities.TryRemove(entityId, out _);
    }
}