﻿//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Extensions;
using NosSmooth.Game.Apis;
using NosSmooth.Game.Apis.Safe;
using NosSmooth.Game.Apis.Unsafe;
using NosSmooth.Game.Contracts;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Inventory;
using NosSmooth.Game.Events.Ui;
using NosSmooth.Game.PacketHandlers.Act4;
using NosSmooth.Game.PacketHandlers.Characters;
using NosSmooth.Game.PacketHandlers.Entities;
using NosSmooth.Game.PacketHandlers.Inventory;
using NosSmooth.Game.PacketHandlers.Map;
using NosSmooth.Game.PacketHandlers.Raids;
using NosSmooth.Game.PacketHandlers.Relations;
using NosSmooth.Game.PacketHandlers.Skills;
using NosSmooth.Game.PacketHandlers.Specialists;
using NosSmooth.Game.PacketHandlers.Ui;
using NosSmooth.Packets.Server.Raids;

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
            .AddManagedNostaleCore()
            .AddMemoryCache()
            .TryAddScoped<EventDispatcher>();
        serviceCollection.TryAddSingleton<Game>();

        serviceCollection

            // act4
            .AddPacketResponder<FcResponder>()

            // character
            .AddPacketResponder<CharacterInitResponder>()
            .AddPacketResponder<WalkResponder>()

            // skills
            .AddPacketResponder<PlayerSkillResponder>()
            .AddPacketResponder<MatesSkillResponder>()
            .AddPacketResponder<SkillUsedResponder>()
            .AddPacketResponder<AoeSkillUsedResponder>()

            // friends
            .AddPacketResponder<FriendInitResponder>()

            // inventory
            .AddPacketResponder<InventoryInitResponder>()

            // groups
            .AddPacketResponder<GroupInitResponder>()

            // mates
            .AddPacketResponder<MatesInitResponder>()
            .AddPacketResponder<PtctlResponder>()

            // map
            .AddPacketResponder<AtResponder>()
            .AddPacketResponder<CMapResponder>()
            .AddPacketResponder<DropResponder>()
            .AddPacketResponder<GpPacketResponder>()
            .AddPacketResponder<InResponder>()
            .AddPacketResponder<MoveResponder>()
            .AddPacketResponder<OutResponder>()
            .AddPacketResponder<RestResponder>()
            .AddPacketResponder<TpResponder>()
            .AddPacketResponder<MapclearResponder>()

            // entities
            .AddPacketResponder<DieResponder>()
            .AddPacketResponder<ReviveResponder>()
            .AddPacketResponder<CharScResponder>()

            // hp, mp
            .AddPacketResponder<StatPacketResponder>()
            .AddPacketResponder<StPacketResponder>()
            .AddPacketResponder<CondPacketResponder>()

            // equip
            .AddPacketResponder<SpResponder>()
            .AddPacketResponder<EqResponder>()

            // raids
            .AddPacketResponder<ThrowResponder>()
            .AddPacketResponder<RaidBfResponder>()
            .AddPacketResponder<RaidMbfResponder>()
            .AddPacketResponder<RaidResponder>()
            .AddPacketResponder<RaidHpMpResponder>()
            .AddPacketResponder<RbossResponder>()
            .AddPacketResponder<RdlstResponder>()

            // ui
            .AddPacketResponder<DialogOpenResponder>();

        serviceCollection
            .AddTransient<DialogHandler>()
            .AddTransient<NostaleMapApi>()
            .AddTransient<NostaleSkillsApi>()
            .AddTransient<NostaleChatApi>()
            .AddTransient<UnsafeMapApi>()
            .AddTransient<UnsafeInventoryApi>()
            .AddTransient<UnsafeMateApi>()
            .AddTransient<UnsafeMateSkillsApi>()
            .AddTransient<UnsafeSkillsApi>();

        serviceCollection
            .AddScoped<IEveryGameResponder, ContractEventResponder>();

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
        if (serviceCollection.Any(x => x.ImplementationType == gameResponder))
        { // already added... assuming every packet responder was added even though that may not be the case.
            return serviceCollection;
        }

        if (!gameResponder.GetInterfaces().Any
            (
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameResponder<>)
            ))
        {
            throw new ArgumentException
            (
                $"{nameof(gameResponder)} should implement IGameResponder.",
                nameof(gameResponder)
            );
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