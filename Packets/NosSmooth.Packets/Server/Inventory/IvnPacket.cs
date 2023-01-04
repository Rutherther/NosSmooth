//
//  IvnPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// An update of inventory.
/// For inventory initialization,
/// <see cref="InvPacket"/>.
/// </summary>
/// <param name="Bag">The bag that should be updated.</param>
/// <param name="InvSubPacket">The data to be updated.</param>
[PacketHeader("ivn", PacketSource.Server)]
[GenerateSerializer(true)]
public record IvnPacket
(
    [PacketIndex(0)]
    BagType Bag,
    [PacketIndex(1, InnerSeparator = '.')]
    InvSubPacket InvSubPacket
) : IPacket;