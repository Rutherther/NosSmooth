//
//  PacketEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes.Packets.Events;

/// <summary>
/// A fake packet event.
/// </summary>
public class PacketEvent : IPreExecutionEvent, IPostExecutionEvent
{
    private readonly Func<INostaleClient, PacketSource, IPacket, string, Result> _preHandler;
    private readonly Func<INostaleClient, PacketSource, IPacket, string, IReadOnlyList<Result>, Result> _postHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketEvent"/> class.
    /// </summary>
    /// <param name="preHandler">The pre handler.</param>
    /// <param name="postHandler">The post handler.</param>
    public PacketEvent
    (
        Func<INostaleClient, PacketSource, IPacket, string, Result> preHandler,
        Func<INostaleClient, PacketSource, IPacket, string, IReadOnlyList<Result>, Result> postHandler
    )
    {
        _preHandler = preHandler;
        _postHandler = postHandler;

    }

    /// <inheritdoc />
    public Task<Result> ExecuteBeforeExecutionAsync<TPacket>
        (INostaleClient client, PacketEventArgs<TPacket> packetArgs, CancellationToken ct = default)
        where TPacket : IPacket
        => Task.FromResult(_preHandler(client, packetArgs.Source, packetArgs.Packet, packetArgs.PacketString));

    /// <inheritdoc />
    public Task<Result> ExecuteBeforeExecutionAsync
        (INostaleClient client, PacketEventArgs packetArgs, CancellationToken ct = default)
        => Task.FromResult(Result.FromSuccess());

    /// <inheritdoc />
    public Task<Result> ExecuteAfterExecutionAsync<TPacket>
    (
        INostaleClient client,
        PacketEventArgs<TPacket> packetArgs,
        IReadOnlyList<Result> executionResults,
        CancellationToken ct = default
    )
        where TPacket : IPacket
        => Task.FromResult
        (
            _postHandler
            (
                client,
                packetArgs.Source,
                packetArgs.Packet,
                packetArgs.PacketString,
                executionResults
            )
        );

    /// <inheritdoc />
    public Task<Result> ExecuteAfterExecutionAsync
    (
        INostaleClient client,
        PacketEventArgs packetArgs,
        IReadOnlyList<Result> executionResults,
        CancellationToken ct = default
    )
        => Task.FromResult(Result.FromSuccess());
}