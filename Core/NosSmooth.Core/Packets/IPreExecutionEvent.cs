//
//  IPreExecutionEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Event executed prior to packet responders.
/// </summary>
public interface IPreExecutionEvent
{
    /// <summary>
    /// Execute the pre execution event.
    /// </summary>
    /// <remarks>
    /// If an error is retuned, the packet responders won't be called.
    /// </remarks>
    /// <param name="client">The NosTale client.</param>
    /// <param name="packetArgs">The packet arguments.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <returns>A result that may or may not succeed.</returns>
    public Task<Result> ExecuteBeforeExecutionAsync<TPacket>
    (
        INostaleClient client,
        PacketEventArgs<TPacket> packetArgs,
        CancellationToken ct = default
    )
        where TPacket : IPacket;

    /// <summary>
    /// Execute the pre execution event.
    /// </summary>
    /// <remarks>
    /// If an error is retuned, the packet responders won't be called.
    /// </remarks>
    /// <param name="client">The NosTale client.</param>
    /// <param name="packetArgs">The packet arguments.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not succeed.</returns>
    public Task<Result> ExecuteBeforeExecutionAsync
    (
        INostaleClient client,
        PacketEventArgs packetArgs,
        CancellationToken ct = default
    );
}