//
//  RawPacketHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Client;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Calls IRawPacketResponder.
/// </summary>
public class RawPacketHandler : IPacketHandler
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="RawPacketHandler"/> class.
    /// </summary>
    /// <param name="services">The serivce provider.</param>
    public RawPacketHandler(IServiceProvider services)
    {
        _services = services;

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
        using var scope = _services.CreateScope();
        var packetEventArgs = new PacketEventArgs(packetType, packetString);

        var preExecutionResult = await ExecuteBeforeExecutionAsync(scope.ServiceProvider, client, packetEventArgs, ct);
        if (!preExecutionResult.IsSuccess)
        {
            return preExecutionResult;
        }

        var packetResponders = scope.ServiceProvider.GetServices<IRawPacketResponder>();

        Result[] results;
        try
        {
            var tasks = packetResponders.Select
                (responder => SafeCall(() => responder.Respond(packetEventArgs, ct))).ToList();

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

    private async Task<Result> ExecuteBeforeExecutionAsync
    (
        IServiceProvider services,
        INostaleClient client,
        PacketEventArgs eventArgs,
        CancellationToken ct
    )
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

    private async Task<Result> ExecuteAfterExecutionAsync
    (
        IServiceProvider services,
        INostaleClient client,
        PacketEventArgs eventArgs,
        IReadOnlyList<Result> executionResults,
        CancellationToken ct
    )
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