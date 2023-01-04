//
//  FInitPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Relations;

/// <summary>
/// Information about friends (and other relations) of a character.
/// </summary>
/// <param name="FriendSubPackets"></param>
[PacketHeader("finit", PacketSource.Server)]
[GenerateSerializer(true)]
public record FInitPacket
(
    [PacketListIndex(0, InnerSeparator = '|', ListSeparator = ' ')]
    IReadOnlyList<FInitSubPacket> FriendSubPackets
) : IPacket;