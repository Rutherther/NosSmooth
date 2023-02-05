//
//  ManagedPacketHandler.cs
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
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Errors;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Calls IRawPacketResponder and IPacketResponder{T}.
/// </summary>
public class ManagedPacketHandler : IPacketHandler
{
    private readonly IServiceProvider _services;
    private readonly IPacketSerializer _packetSerializer;
    private readonly ILogger<ManagedPacketHandler> _logger;
    private readonly IPacketHandler _rawPacketHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedPacketHandler"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="packetSerializer">The packet serializer.</param>
    /// <param name="logger">The logger.</param>
    public ManagedPacketHandler
        (IServiceProvider services, IPacketSerializer packetSerializer, ILogger<ManagedPacketHandler> logger)
    {
        _rawPacketHandler = new RawPacketHandler(services);
        _services = services;
        _packetSerializer = packetSerializer;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> HandlePacketAsync
    (
        INostaleClient client,
        PacketSource packetType,
        string packetString,
        CancellationToken ct = default
    )
    {
        var rawResult = await _rawPacketHandler.HandlePacketAsync(client, packetType, packetString, ct);

        IPacket packet;
        var deserializedResult = _packetSerializer.Deserialize(packetString, packetType);
        if (!deserializedResult.IsDefined(out var _))
        {
            if (deserializedResult.Error is not PacketConverterNotFoundError)
            {
                _logger.LogWarning("Could not parse {Packet}. Reason:", packetString);
                _logger.LogResultError(deserializedResult);
                packet = new ParsingFailedPacket(deserializedResult, packetString);
            }
            else
            {
                packet = new UnresolvedPacket(packetString.Split(' ')[0], packetString);
            }
        }
        else
        {
            packet = deserializedResult.Entity;
        }

        var managedResult = await HandlePacketAsync
        (
            client,
            packetType,
            packet,
            packetString,
            ct
        );

        if (!rawResult.IsSuccess && !managedResult.IsSuccess)
        {
            return new AggregateError(rawResult, managedResult);
        }

        if (!rawResult.IsSuccess)
        {
            return rawResult;
        }

        if (!managedResult.IsSuccess)
        {
            return managedResult;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Calls a responder for the given packet.
    /// </summary>
    /// <param name="client">The current NosTale client.</param>
    /// <param name="packetType">The source of the packet.</param>
    /// <param name="packet">The packet.</param>
    /// <param name="packetString">The string of the packet.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    private Task<Result> HandlePacketAsync
    (
        INostaleClient client,
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
        return (Task<Result>)boundProcessMethod.Invoke
        (
            this,
            new object[]
            {
                client,
                packetType,
                packet,
                packetString,
                ct
            }
        )!;
    }

    private async Task<Result> DispatchResponder<TPacket>
    (
        INostaleClient client,
        PacketSource packetType,
        TPacket packet,
        string packetString,
        CancellationToken ct
    )
        where TPacket : class, IPacket
    {
        using var scope = _services.CreateScope();
        var packetEventArgs = new PacketEventArgs<TPacket>(packetType, packet, packetString);

        var preExecutionResult = await ExecuteBeforeExecutionAsync(scope.ServiceProvider, client, packetEventArgs, ct);
        if (!preExecutionResult.IsSuccess)
        {
            return preExecutionResult;
        }

        var packetResponders = scope.ServiceProvider.GetServices<IPacketResponder<TPacket>>();
        var genericPacketResponders = scope.ServiceProvider.GetServices<IEveryPacketResponder>();

        Result[] results;
        try
        {
            var tasks = packetResponders.Select
                (responder => SafeCall(() => responder.Respond(packetEventArgs, ct))).ToList();
            tasks.AddRange
                (genericPacketResponders.Select(responder => SafeCall(() => responder.Respond(packetEventArgs, ct))));

            results = await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            results = new Result[] { e };
        }

        var errors = new List<Result>();
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                errors.Add(result);
            }
        }

        var postExecutionResult = await ExecuteAfterExecutionAsync
        (
            scope.ServiceProvider,
            client,
            packetEventArgs,
            results,
            ct
        );
        if (!postExecutionResult.IsSuccess)
        {
            errors.Add(postExecutionResult);
        }

        return errors.Count switch
        {
            0 => Result.FromSuccess(),
            1 => errors[0],
            _ => new AggregateError(errors.Cast<IResult>().ToArray())
        };
    }

    private async Task<Result> ExecuteBeforeExecutionAsync<TPacket>
    (
        IServiceProvider services,
        INostaleClient client,
        PacketEventArgs<TPacket> eventArgs,
        CancellationToken ct
    )
        where TPacket : IPacket
    {
        try
        {
            var results = await Task.WhenAll
            (
                services.GetServices<IPreExecutionEvent>()
                    .Select(x => SafeCall(() => x.ExecuteBeforeExecutionAsync(client, eventArgs, ct)))
            );

            var errorResults = new List<Result>();
            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    errorResults.Add(result);
                }
            }

            return errorResults.Count switch
            {
                1 => errorResults[0],
                0 => Result.FromSuccess(),
                _ => new AggregateError(errorResults.Cast<IResult>().ToArray())
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private async Task<Result> ExecuteAfterExecutionAsync<TPacket>
    (
        IServiceProvider services,
        INostaleClient client,
        PacketEventArgs<TPacket> eventArgs,
        IReadOnlyList<Result> executionResults,
        CancellationToken ct
    )
        where TPacket : IPacket
    {
        try
        {
            var results = await Task.WhenAll
            (
                services.GetServices<IPostExecutionEvent>()
                    .Select(x => SafeCall(() => x.ExecuteAfterExecutionAsync(client, eventArgs, executionResults, ct)))
            );

            var errorResults = new List<Result>();
            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    errorResults.Add(result);
                }
            }

            return errorResults.Count switch
            {
                1 => errorResults[0],
                0 => Result.FromSuccess(),
                _ => new AggregateError(errorResults.Cast<IResult>().ToArray())
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private async Task<Result> SafeCall(Func<Task<Result>> task)
    {
        try
        {
            return await task();
        }
        catch (Exception e)
        {
            return e;
        }
    }
}