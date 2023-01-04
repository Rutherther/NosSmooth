//
//  FInfoPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Relations;

/// <summary>
/// Information about friends of a character.
/// </summary>
/// <param name="FriendSubPackets">The friends list.</param>
[PacketHeader("finfo", PacketSource.Server)]
[GenerateSerializer(true)]
public record FInfoPacket
(
    [PacketListIndex(0, InnerSeparator = '.', ListSeparator = ' ')]
    IReadOnlyList<FInfoSubPacket> FriendSubPackets
) : IPacket;