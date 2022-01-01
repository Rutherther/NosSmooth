//
//  InItemSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Entities;

/// <summary>
/// Sub packet of <see cref="InPacket"/> present if the in packet
/// is for an object.
/// </summary>
/// <param name="Amount">The amount of the gold or drop etc.</param>
/// <param name="IsQuestRelative">Whether the item is needed for a quest.</param>
/// <param name="OwnerId">The id of the owner of the item.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record InItemSubPacket
(
    [PacketIndex(0)]
    long Amount,
    [PacketIndex(1)]
    bool IsQuestRelative,
    [PacketIndex(2)]
    long OwnerId
) : IPacket;