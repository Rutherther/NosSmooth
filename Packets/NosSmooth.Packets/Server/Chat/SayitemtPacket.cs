//
//  SayitemtPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Chat;

/// <summary>
/// Information about an item sent to chat.
/// </summary>
/// <param name="EntityType">The type of the entity that spoke.</param>
/// <param name="EntityId">The id of the entity that spoke.</param>
/// <param name="Color">The say/message color.</param>
/// <param name="Amount">The amount.</param>
/// <param name="ItemVNum">The vnum of the item.</param>
/// <param name="Name">The name of the item.</param>
/// <param name="Message">The message template. Usully "{%s%}"</param>
/// <param name="ItemInfo">TODO information about an item, e_info packet.</param>
[PacketHeader("sayitemt", PacketSource.Server)]
[GenerateSerializer(true)]
public record SayitemtPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    SayColor Color,
    [PacketIndex(3)]
    short Amount,
    [PacketIndex(4)]
    int ItemVNum,
    [PacketIndex(5)]
    NameString Name,
    [PacketIndex(6)]
    string Message,
    [PacketGreedyIndex(7)]
    string ItemInfo
) : IPacket;