//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Commands.Control;
using NosSmooth.Core.Packets;
using NosSmooth.Packets.Extensions;

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
            .TryAddSingleton<IPacketHandler, PacketHandler>();

        serviceCollection.AddPacketSerialization();
        serviceCollection.AddSingleton<CommandProcessor>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds command handling of <see cref="TakeControlCommand"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddTakeControlCommand(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ControlCommands>()
            .AddPacketResponder<ControlCommandPacketResponders>()
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
        if (responderType.GetInterfaces().Any(i => i == typeof(IEveryPacketResponder)))
        {
            return serviceCollection.AddScoped(typeof(IEveryPacketResponder), responderType);
        }

        if (!responderType.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPacketResponder<>)
            ))
        {
            throw new ArgumentException(
                $"{nameof(responderType)} should implement IPacketResponder.",
                nameof(responderType));
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
        if (!commandHandlerType.GetInterfaces().Any(
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
            ))
        {
            throw new ArgumentException(
                $"{nameof(commandHandlerType)} should implement ICommandHandler.",
                nameof(commandHandlerType));
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
}
