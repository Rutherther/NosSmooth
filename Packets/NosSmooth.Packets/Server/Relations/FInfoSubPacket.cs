//
//  FInfoSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Relations;

/// <summary>
/// A sub packet of <see cref="FInfoPacket"/>
/// containing information about a friend.
/// </summary>
/// <param name="PlayerId">The id of the player.</param>
/// <param name="IsConnected">Whether the player is connected.</param>
/// <param name="Name">The name of the player.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record FInfoSubPacket
(
    [PacketIndex(0)]
    long PlayerId,
    [PacketIndex(1)]
    bool IsConnected,
    [PacketIndex(2)]
    NameString Name
);