//
//  InvPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// Contents of inventory bag.
/// </summary>
/// <remarks>
/// This is sent at startup.
/// For updates, <see cref="IvnPacket"/>,
/// that is sent after changes.
/// </remarks>
/// <param name="Bag">The bag that the items are in.</param>
/// <param name="InvSubPackets">The items inside the bag.</param>
[PacketHeader("inv", PacketSource.Server)]
[GenerateSerializer(true)]
public record InvPacket
(
    [PacketIndex(0)]
    BagType Bag,
    [PacketListIndex(1, InnerSeparator = '.', ListSeparator = ' ', IsOptional = true)]
    IReadOnlyList<InvSubPacket>? InvSubPackets
) : IPacket;