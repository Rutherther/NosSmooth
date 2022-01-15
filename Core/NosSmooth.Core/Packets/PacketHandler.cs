//
//  PacketHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <inheritdoc />
public class PacketHandler : IPacketHandler
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketHandler"/> class.
    /// </summary>
    /// <param name="provider">The dependency injection provider.</param>
    public PacketHandler(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public Task<Result> HandleReceivedPacketAsync(IPacket packet, string packetString, CancellationToken ct)
        => HandlePacketAsync(PacketSource.Server, packet, packetString, ct);

    /// <inheritdoc />
    public Task<Result> HandleSentPacketAsync(IPacket packet, string packetString, CancellationToken ct)
        => HandlePacketAsync(PacketSource.Client, packet, packetString, ct);

    private Task<Result> HandlePacketAsync
    (
        PacketSource packetType,
        IPacket packet,
        string packetString,
        CancellationToken ct = default
    )
    {
        var processMethod = GetType().GetMethod
        (
            nameof(DispatchResponder),
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (processMethod is null)
        {
            throw new InvalidOperationException("Could not find process command generic method in command processor.");
        }

        var boundProcessMethod = processMethod.MakeGenericMethod(packet.GetType());
        return (Task<Result>)boundProcessMethod.Invoke(this, new object[]
        {
            packetType,
            packet,
            packetString,
            ct
        })!;
    }

    private async Task<Result> DispatchResponder<TPacket>(
        PacketSource packetType,
        TPacket packet,
        string packetString,
        CancellationToken ct
    )
        where TPacket : class, IPacket
    {
        using var scope = _provider.CreateScope();
        var packetResponders = scope.ServiceProvider.GetServices<IPacketResponder<TPacket>>();
        var genericPacketResponders = scope.ServiceProvider.GetServices<IEveryPacketResponder>();

        var packetEventArgs = new PacketEventArgs<TPacket>(packetType, packet, packetString);
        var tasks = packetResponders.Select(responder => responder.Respond(packetEventArgs, ct)).ToList();
        tasks.AddRange(genericPacketResponders.Select(responder => responder.Respond(packetEventArgs, ct)));

        var results = await Task.WhenAll(tasks);

        var errors = new List<Result>();
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                errors.Add(result);
            }
        }

        return errors.Count switch
        {
            0 => Result.FromSuccess(),
            1 => errors[0],
            _ => new AggregateError(errors.Cast<IResult>().ToArray())
        };
    }
}
