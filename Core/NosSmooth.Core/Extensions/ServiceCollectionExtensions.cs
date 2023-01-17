//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Commands.Control;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Contracts;
using NosSmooth.Core.Contracts.Responders;
using NosSmooth.Core.Packets;
using NosSmooth.Core.Stateful;
using NosSmooth.PacketSerializer.Extensions;

namespace NosSmooth.Core.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds base packet and command handling for nostale client.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleCore
    (
        this IServiceCollection serviceCollection
    )
    {
        serviceCollection
            .TryAddSingleton<PacketHandler>();

        serviceCollection.AddPacketSerialization();
        serviceCollection.AddSingleton<CommandProcessor>();

        serviceCollection
            .AddSingleton<Contractor>()
            .AddPacketResponder<ContractPacketResponder>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds command handling of <see cref="TakeControlCommand"/> and <see cref="WalkCommand"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddTakeControlCommand(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ControlCommands>()
            .AddPacketResponder<ControlCommandPacketResponders>()
            .AddCommandHandler<WalkCommandHandler>()
            .AddCommandHandler<TakeControlCommandHandler>();
    }

    /// <summary>
    /// Adds the specified packet responder that will be called upon receiving the given event.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <typeparam name="TPacketResponder">The type of the responder.</typeparam>
    /// <exception cref="ArgumentException">Thrown if the type of the responder is incorrect.</exception>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPacketResponder<TPacketResponder>
    (
        this IServiceCollection serviceCollection
    )
        where TPacketResponder : class, IPacketResponder
    {
        return serviceCollection.AddPacketResponder(typeof(TPacketResponder));
    }

    /// <summary>
    /// Adds the specified packet responder that will be called upon receiving the given event.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <param name="responderType">The type of the responder.</param>
    /// <returns>The collection.</returns>
    /// <exception cref="ArgumentException">Thrown if the type of the responder is incorrect.</exception>
    public static IServiceCollection AddPacketResponder
    (
        this IServiceCollection serviceCollection,
        Type responderType
    )
    {
        if (serviceCollection.Any(x => x.ImplementationType == responderType))
        { // already added... assuming every packet responder was added even though that may not be the case.
            return serviceCollection;
        }

        if (responderType.GetInterfaces().Any(i => i == typeof(IEveryPacketResponder)))
        {
            serviceCollection.AddScoped(typeof(IEveryPacketResponder), responderType);
        }

        if (!responderType.GetInterfaces().Any
            (
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPacketResponder<>)
            ))
        {
            throw new ArgumentException
            (
                $"{nameof(responderType)} should implement IPacketResponder.",
                nameof(responderType)
            );
        }

        var responderTypeInterfaces = responderType.GetInterfaces();
        var responderInterfaces = responderTypeInterfaces.Where
        (
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IPacketResponder<>)
        );

        foreach (var responderInterface in responderInterfaces)
        {
            serviceCollection.AddScoped(responderInterface, responderType);
        }

        return serviceCollection;
    }

    /// <summary>
    /// Adds the specified command handler.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <typeparam name="TCommandHandler">The type of the command.</typeparam>
    /// <exception cref="ArgumentException">Thrown if the type of the responder is incorrect.</exception>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddCommandHandler<TCommandHandler>
    (
        this IServiceCollection serviceCollection
    )
        where TCommandHandler : class, ICommandHandler
    {
        return serviceCollection.AddCommandHandler(typeof(TCommandHandler));
    }

    /// <summary>
    /// Adds the specified command handler.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <param name="commandHandlerType">The type of the command handler.</param>
    /// <returns>The collection.</returns>
    /// <exception cref="ArgumentException">Thrown if the type of the responder is incorrect.</exception>
    public static IServiceCollection AddCommandHandler
    (
        this IServiceCollection serviceCollection,
        Type commandHandlerType
    )
    {
        if (!commandHandlerType.GetInterfaces().Any
            (
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
            ))
        {
            throw new ArgumentException
            (
                $"{nameof(commandHandlerType)} should implement ICommandHandler.",
                nameof(commandHandlerType)
            );
        }

        var handlerTypeInterfaces = commandHandlerType.GetInterfaces();
        var handlerInterfaces = handlerTypeInterfaces.Where
        (
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
        );

        foreach (var handlerInterface in handlerInterfaces)
        {
            serviceCollection.AddScoped(handlerInterface, commandHandlerType);
        }

        return serviceCollection;
    }

    /// <summary>
    /// Add the given pre execution event that will be executed before the command handler.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The pre execution event type.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPreCommandExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPreCommandExecutionEvent
    {
        return serviceCollection.AddScoped<IPreCommandExecutionEvent, TEvent>();
    }

    /// <summary>
    /// Add the given post execution event that will be executed after the command handler.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The pre execution event type.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPostCommandExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPostCommandExecutionEvent
    {
        return serviceCollection.AddScoped<IPostCommandExecutionEvent, TEvent>();
    }

    /// <summary>
    /// Add the given pre execution event that will be executed before the packet responders.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The pre execution event type.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPreExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPreExecutionEvent
    {
        return serviceCollection.AddScoped<IPreExecutionEvent, TEvent>();
    }

    /// <summary>
    /// Add the given post execution event that will be executed after the packet responders.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEvent">The pre execution event type.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPostExecutionEvent<TEvent>(this IServiceCollection serviceCollection)
        where TEvent : class, IPostExecutionEvent
    {
        return serviceCollection.AddScoped<IPostExecutionEvent, TEvent>();
    }

    /// <summary>
    /// Add the injector for stateful entities that may be replaced for different NosTale clients.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddStatefulInjector(this IServiceCollection serviceCollection)
    {
        serviceCollection.RemoveAll<INostaleClient>();
        return serviceCollection
            .AddScoped<StatefulInjector>()
            .AddSingleton<StatefulRepository>()
            .AddPreExecutionEvent<StatefulPreExecutionEvent>()
            .AddPreCommandExecutionEvent<StatefulPreExecutionEvent>()
            .AddScoped<INostaleClient>
            (
                p =>
                {
                    var nostaleClient = p.GetRequiredService<StatefulInjector>().Client;
                    if (nostaleClient == null)
                    {
                        throw new NullReferenceException("The client cannot be null.");
                    }

                    return nostaleClient;
                }
            )
            .ReplaceStatefulEntities();
    }

    /// <summary>
    /// Replace all the stateful entities that are added in the collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection ReplaceStatefulEntities(this IServiceCollection serviceCollection)
    {
        foreach (var serviceDescriptor in serviceCollection)
        {
            var type = serviceDescriptor.ServiceType;
            if (typeof(IStatefulEntity).IsAssignableFrom(type))
            {
                serviceCollection.AddStatefulEntity(type);
            }
        }

        return serviceCollection;
    }

    /// <summary>
    /// Add a stateful entity of the given type to be injectable into the scope of a client.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TEntity">The type of the stateful entity.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddStatefulEntity<TEntity>(this IServiceCollection serviceCollection)
        => serviceCollection.AddStatefulEntity(typeof(TEntity));

    /// <summary>
    /// Add a stateful entity of the given type to be injectable into the scope of a client.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="statefulEntityType">The type of the stateful entity.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddStatefulEntity
        (this IServiceCollection serviceCollection, Type statefulEntityType)
    {
        serviceCollection.AddStatefulInjector();
        serviceCollection.RemoveAll(statefulEntityType);
        serviceCollection.Add
        (
            ServiceDescriptor.Scoped
                (statefulEntityType, (p) => p.GetRequiredService<StatefulInjector>().GetEntity(p, statefulEntityType))
        );

        return serviceCollection;
    }
}