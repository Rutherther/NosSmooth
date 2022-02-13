//
//  StatefulInjector.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Core.Client;

namespace NosSmooth.Core.Stateful;

/// <summary>
/// The scoped injector of stateful entities.
/// </summary>
public class StatefulInjector
{
    private readonly StatefulRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulInjector"/> class.
    /// </summary>
    /// <param name="repository">The repository of stateful entity types.</param>
    public StatefulInjector(StatefulRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Gets or sets the nostale client.
    /// </summary>
    public INostaleClient? Client { get; set; }

    /// <summary>
    /// Gets an entity of the specified type.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="statefulEntityType">The type of the entity.</param>
    /// <exception cref="NullReferenceException">Thrown if the client is not set.</exception>
    /// <returns>The entity to inject.</returns>
    public object GetEntity(IServiceProvider services, Type statefulEntityType)
    {
        if (Client is null)
        {
            throw new NullReferenceException("The client cannot be null in order to get a stateful entity.");
        }

        return _repository.GetEntity(services, Client, statefulEntityType);
    }
}