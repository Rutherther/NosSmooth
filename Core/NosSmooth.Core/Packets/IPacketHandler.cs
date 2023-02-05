//
//  IPacketHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Calls registered responders for the packet that should be handled.
/// </summary>
public interface IPacketHandler
{
    /// <summary>
    /// Calls a responder for the given packet.
    /// </summary>
    /// <param name="client">The current NosTale client.</param>
    /// <param name="packetType">The source of the packet.</param>
    /// <param name="packetString">The string of the packet.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> HandlePacketAsync
    (
        INostaleClient client,
        PacketSource packetType,
        string packetString,
        CancellationToken ct = default
    );
}