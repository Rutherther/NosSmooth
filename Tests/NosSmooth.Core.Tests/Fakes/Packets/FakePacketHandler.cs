//
//  FakePacketHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes.Packets;

/// <summary>
/// Fake Responder of a packet.
/// </summary>
/// <typeparam name="TPacket">The packet to respond to.</typeparam>
public class FakePacketResponder<TPacket> : IPacketResponder<TPacket>
    where TPacket : IPacket
{
    private readonly Func<PacketEventArgs<TPacket>, Result> _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakePacketResponder{TPacket}"/> class.
    /// </summary>
    /// <param name="handler">The function respond handler.</param>
    public FakePacketResponder(Func<PacketEventArgs<TPacket>, Result> handler)
    {
        _handler = handler;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<TPacket> packetArgs, CancellationToken ct = default)
        => Task.FromResult(_handler(packetArgs));
}