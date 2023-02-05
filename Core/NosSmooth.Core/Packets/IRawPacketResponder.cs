//
//  IRawPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Represents interface for classes that respond to packets.
/// Responds to a raw packet string.
/// </summary>
public interface IRawPacketResponder
{
    /// <summary>
    /// Respond to the given packet.
    /// </summary>
    /// <param name="packetArgs">The packet to respond to.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> Respond(PacketEventArgs packetArgs, CancellationToken ct = default);
}