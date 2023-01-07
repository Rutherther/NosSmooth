//
//  GetPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Inventory;

/// <summary>
/// Pick up an item from ground.
/// </summary>
/// <param name="PickerEntityType">The type of the picker entity.</param>
/// <param name="PickerEntityId">The id of the picker entity.</param>
/// <param name="GroundItemId">The id of the ground item to pick up.</param>
[PacketHeader("get", PacketSource.Client)]
[GenerateSerializer(true)]
public record GetPacket
(
    [PacketIndex(0)]
    EntityType PickerEntityType,
    [PacketIndex(1)]
    long PickerEntityId,
    [PacketIndex(2)]
    long GroundItemId
) : IPacket;