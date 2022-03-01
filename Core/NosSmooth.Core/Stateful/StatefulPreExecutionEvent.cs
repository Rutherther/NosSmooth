//
//  StatefulPreExecutionEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Stateful;

/// <summary>
/// Event that injects stateful entities into the scope.
/// </summary>
internal class StatefulPreExecutionEvent : IPreExecutionEvent, IPreCommandExecutionEvent
{
    private readonly StatefulInjector _injector;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatefulPreExecutionEvent"/> class.
    /// </summary>
    /// <param name="injector">The injector.</param>
    public StatefulPreExecutionEvent(StatefulInjector injector)
    {
        _injector = injector;
    }

    /// <inheritdoc />
    public Task<Result> ExecuteBeforeExecutionAsync<TPacket>
        (INostaleClient client, PacketEventArgs<TPacket> packetArgs, CancellationToken ct = default)
        where TPacket : IPacket
    {
        _injector.Client = client;
        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc />
    public Task<Result> ExecuteBeforeCommandAsync<TCommand>(INostaleClient client, TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        _injector.Client = client;
        return Task.FromResult(Result.FromSuccess());
    }
}