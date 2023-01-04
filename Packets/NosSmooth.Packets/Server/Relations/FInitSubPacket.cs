//
//  FInitSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Relations;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Relations;

/// <summary>
/// A sub packet of <see cref="FInitSubPacket"/>
/// containing information about a friend.
/// </summary>
/// <param name="PlayerId">The id of the friend.</param>
/// <param name="RelationType">The relation between character-player.</param>
/// <param name="IsConnected">Whether the friend is connected.</param>
/// <param name="Name">The name of the friend.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record FInitSubPacket
(
    [PacketIndex(0)]
    long PlayerId,
    [PacketIndex(1)]
    CharacterRelationType? RelationType,
    [PacketIndex(2)]
    bool IsConnected,
    [PacketIndex(3)]
    NameString Name
) : IPacket;