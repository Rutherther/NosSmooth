//
//  DropPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// An item drop packet.
/// </summary>
/// <param name="ItemVNum">The item vnum identifier.</param>
/// <param name="DropId">The drop id.</param>
/// <param name="X">The map x.</param>
/// <param name="Y">The map y.</param>
/// <param name="Amount">The amount of the items on the ground.</param>
/// <param name="IsQuestRelated">Whether the item is a quest related item.</param>
/// <param name="OwnerId">The id of the owner. (only the owner is able to pick up the item)</param>
[PacketHeader("drop", PacketSource.Server)]
[GenerateSerializer(true)]
public record DropPacket
(
    [PacketIndex(0)]
    int ItemVNum,
    [PacketIndex(1)]
    long DropId,
    [PacketIndex(2)]
    short X,
    [PacketIndex(3)]
    short Y,
    [PacketIndex(4)]
    short Amount,
    [PacketIndex(5)]
    bool IsQuestRelated,
    [PacketIndex(6)]
    long? OwnerId
) : IPacket;