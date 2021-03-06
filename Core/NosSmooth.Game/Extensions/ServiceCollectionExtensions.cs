//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Extensions;
using NosSmooth.Game.Apis;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.PacketHandlers.Characters;
using NosSmooth.Game.PacketHandlers.Entities;
using NosSmooth.Game.PacketHandlers.Map;
using NosSmooth.Game.PacketHandlers.Specialists;

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
            .TryAddScoped<EventDispatcher>();
        serviceCollection.TryAddSingleton<Game>();

        serviceCollection
            .AddPacketResponder<CharacterInitResponder>()
            .AddPacketResponder<SkillResponder>()
            .AddPacketResponder<WalkResponder>()
            .AddPacketResponder<SkillUsedResponder>()
            .AddPacketResponder<AoeSkillUsedResponder>()
            .AddPacketResponder<AtResponder>()
            .AddPacketResponder<CMapResponder>()
            .AddPacketResponder<DropResponder>()
            .AddPacketResponder<GpPacketResponder>()
            .AddPacketResponder<InResponder>()
            .AddPacketResponder<MoveResponder>()
            .AddPacketResponder<OutResponder>()
            .AddPacketResponder<StatPacketResponder>()
            .AddPacketResponder<StPacketResponder>()
            .AddPacketResponder<CondPacketResponder>()
            .AddPacketResponder<SpResponder>()
            .AddPacketResponder<EqResponder>();

        serviceCollection
            .AddTransient<NostaleChatPacketApi>()
            .AddTransient<NostaleSkillsPacketApi>();

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
