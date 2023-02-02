//
//  StatefulRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Client;

namespace NosSmooth.Core.Stateful;

/// <summary>
/// Repository holding all the stateful entities for various NosTale clients.
/// </summary>
public class StatefulRepository
{
    private readonly ConcurrentDictionary<INostaleClient, ConcurrentDictionary<Type, object>> _statefulEntities;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulRepository"/> class.
    /// </summary>
    public StatefulRepository()
    {
        _statefulEntities = new ConcurrentDictionary<INostaleClient, ConcurrentDictionary<Type, object>>();
    }

    /// <summary>
    /// Remove items of the given client.
    /// </summary>
    /// <param name="client">The client to remove.</param>
    public void Remove(INostaleClient client)
    {
        _statefulEntities.Remove(client, out _);
    }

    /// <summary>
    /// Set entity of the given type to the given client.
    /// </summary>
    /// <remarks>
    /// If the entity is not set manually, there will be an attempt to create an instance.
    /// </remarks>
    /// <param name="client">The nostale client.</param>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public void SetEntity<TEntity>(INostaleClient client, TEntity entity)
        where TEntity : notnull
    {
        _statefulEntities.TryAdd(client, new ConcurrentDictionary<Type, object>());
        var values = _statefulEntities[client];

        values.AddOrUpdate(typeof(TEntity), (k) => entity, (k, v) => entity);
    }

    /// <summary>
    /// Get an entity for the given client and type.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="client">The NosTale client.</param>
    /// <param name="statefulEntityType">The type of the stateful entity to obtain.</param>
    /// <returns>The obtained entity.</returns>
    public object GetEntity(IServiceProvider services, INostaleClient client, Type statefulEntityType)
    {
        var dict = _statefulEntities.AddOrUpdate
        (
            client,
            _ =>
            {
                var objectDictionary = new ConcurrentDictionary<Type, object>();
                objectDictionary.TryAdd
                    (statefulEntityType, ActivatorUtilities.CreateInstance(services, statefulEntityType));
                return objectDictionary;
            },
            (_, objectDictionary) =>
            {
                if (!objectDictionary.ContainsKey(statefulEntityType))
                {
                    objectDictionary.TryAdd
                        (statefulEntityType, ActivatorUtilities.CreateInstance(services, statefulEntityType));
                }

                return objectDictionary;
            }
        );

        return dict[statefulEntityType];
    }
}