//
//  IPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Represents interface for classes that respond to packets.
/// </summary>
public interface IPacketResponder
{
}

/// <summary>
/// Represents interface for classes that respond to packets.
/// Responds to <typeparamref name="TPacket"/>.
/// </summary>
/// <typeparam name="TPacket">The packet type this responder responds to.</typeparam>
public interface IPacketResponder<TPacket> : IPacketResponder
    where TPacket : IPacket
{
    /// <summary>
    /// Respond to the given packet.
    /// </summary>
    /// <param name="packet">The packet to respond to.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> Respond(PacketEventArgs<TPacket> packet, CancellationToken ct = default);
}

/// <summary>
/// Represents interface for classes that respond to every type of packets.
/// </summary>
public interface IEveryPacketResponder : IPacketResponder
{
    /// <summary>
    /// Respond to the given packet.
    /// </summary>
    /// <param name="packet">The packet to respond to.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> Respond<TPacket>(PacketEventArgs<TPacket> packet, CancellationToken ct = default)
        where TPacket : IPacket;
}