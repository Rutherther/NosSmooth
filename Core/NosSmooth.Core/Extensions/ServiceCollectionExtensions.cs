using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosCore.Packets;
using NosCore.Packets.Attributes;
using NosCore.Packets.Enumerations;
using NosCore.Packets.Interfaces;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;

namespace NosSmooth.Core.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds base packet and command handling for nostale client.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <param name="additionalPacketTypes">Custom types of packets to serialize and deserialize.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleCore(this IServiceCollection serviceCollection,
        params Type[] additionalPacketTypes)
    {
        serviceCollection
            .TryAddSingleton<IPacketHandler, PacketHandler>();

        var clientPacketTypes = typeof(IPacket).Assembly.GetTypes()
            .Where(p => (p.Namespace?.Contains("Client") ?? false) && p.GetInterfaces().Contains(typeof(IPacket)) && p.IsClass && !p.IsAbstract).ToList();
        var serverPacketTypes = typeof(IPacket).Assembly.GetTypes()
            .Where(p => (p.Namespace?.Contains("Server") ?? false) && p.GetInterfaces().Contains(typeof(IPacket)) && p.IsClass && !p.IsAbstract).ToList();

        if (additionalPacketTypes.Length != 0)
        {
            clientPacketTypes.AddRange(additionalPacketTypes);
        }

        serviceCollection.AddSingleton(_ =>
            new PacketSerializerProvider(clientPacketTypes, serverPacketTypes));
        serviceCollection.AddSingleton(p => p.GetRequiredService<PacketSerializerProvider>().GetServerSerializer());

        serviceCollection.AddSingleton<CommandProcessor>();

        return serviceCollection;
    }

    /// <summary>
    /// Adds the specified packet responder that will be called upon receiving the given event.
    /// </summary>
    /// <param name="serviceCollection">The service collection to register the responder to.</param>
    /// <typeparam name="TPacketResponder">The type of the responder.</typeparam>
    /// <exception cref="ArgumentException">Thrown if the type of the responder is incorrect.</exception>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddPacketResponder<TPacketResponder>(
        this IServiceCollection serviceCollection)
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
    public static IServiceCollection AddPacketResponder(this IServiceCollection serviceCollection, Type responderType)
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
    public static IServiceCollection AddCommandHandler<TCommandHandler>(
        this IServiceCollection serviceCollection)
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
    public static IServiceCollection AddCommandHandler(this IServiceCollection serviceCollection, Type commandHandlerType)
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