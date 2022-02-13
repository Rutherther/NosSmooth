//
//  IPostExecutionEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Event executed after the packet responders.
/// </summary>
public interface IPostExecutionEvent
{
    /// <summary>
    /// Execute the post execution event.
    /// </summary>
    /// <param name="client">The NosTale client.</param>
    /// <param name="packetArgs">The packet arguments.</param>
    /// <param name="executionResults">The results from the packet responders.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <returns>A result that may or may not succeed.</returns>
    public Task<Result> ExecuteAfterExecutionAsync<TPacket>
    (
        INostaleClient client,
        PacketEventArgs<TPacket> packetArgs,
        IReadOnlyList<Result> executionResults,
        CancellationToken ct = default
    )
        where TPacket : IPacket;
}