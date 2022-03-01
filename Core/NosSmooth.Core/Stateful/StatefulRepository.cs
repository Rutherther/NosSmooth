//
//  StatefulRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Client;

namespace NosSmooth.Core.Stateful;

/// <summary>
/// Repository holding all the stateful entities for various NosTale clients.
/// </summary>
internal class StatefulRepository
{
    private readonly Dictionary<INostaleClient, Dictionary<Type, object>> _statefulEntities;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulRepository"/> class.
    /// </summary>
    public StatefulRepository()
    {
        _statefulEntities = new Dictionary<INostaleClient, Dictionary<Type, object>>();
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
        if (!_statefulEntities.ContainsKey(client))
        {
            _statefulEntities.Add(client, new Dictionary<Type, object>());
        }

        var value = _statefulEntities[client];
        if (!value.ContainsKey(statefulEntityType))
        {
            value.Add(statefulEntityType, ActivatorUtilities.CreateInstance(services, statefulEntityType));
        }

        return value[statefulEntityType];
    }
}