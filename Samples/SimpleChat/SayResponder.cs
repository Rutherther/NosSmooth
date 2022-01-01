//
//  SayResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Packets.Server.Chat;
using Remora.Results;

namespace SimpleChat;

/// <summary>
/// Responds to <see cref="SayPacket"/>.
/// </summary>
public class SayResponder : IPacketResponder<SayPacket>, IPacketResponder<MsgPacket>
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SayResponder"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    public SayResponder(INostaleClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<SayPacket> packet, CancellationToken ct = default)
    {
        return _client.ReceivePacketAsync
        (
            new SayPacket(EntityType.Map, 1, SayColor.Red, "Hello world from NosSmooth!"),
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<MsgPacket> packet, CancellationToken ct = default)
    {
        return _client.ReceivePacketAsync
        (
            new SayPacket(EntityType.Map, 1, SayColor.Red, "Hello world from NosSmooth!"),
            ct
        );
    }
}