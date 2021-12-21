//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Extensions;
using NosSmooth.Game.Events.Handlers;

namespace NosSmooth.Game.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds handling of nostale packets, registering <see cref="Game"/> singleton and dispatching of game events.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleGame(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddNostaleCore()
            .AddMemoryCache()
            .TryAddSingleton<EventDispatcher>();
        serviceCollection.TryAddSingleton<Game>();

        // TODO: add events
        return serviceCollection;
    }

    /// <summary>
    /// Adds the given game event responder.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TGameResponder">The responder to add.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddGameResponder<TGameResponder>(this IServiceCollection serviceCollection)
        where TGameResponder : IGameResponder
    {
        return serviceCollection.AddGameResponder(typeof(TGameResponder));
    }

    /// <summary>
    /// Adds the given game event responder.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="gameResponder">The type of the event responder.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddGameResponder(this IServiceCollection serviceCollection, Type gameResponder)
    {
        if (!gameResponder.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameResponder<>)
            ))
        {
            throw new ArgumentException(
                $"{nameof(gameResponder)} should implement IGameResponder.",
                nameof(gameResponder));
        }

        var handlerTypeInterfaces = gameResponder.GetInterfaces();
        var handlerInterfaces = handlerTypeInterfaces.Where
        (
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IGameResponder<>)
        );

        foreach (var handlerInterface in handlerInterfaces)
        {
            serviceCollection.AddScoped(handlerInterface, gameResponder);
        }

        return serviceCollection;
    }
}
