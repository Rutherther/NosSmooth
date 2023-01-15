//
//  RaidPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Raids;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// Raid status packet. Sent for various raid operations.
/// </summary>
/// <remarks>
/// For every type the rest of the packet is different.
/// Fields start with the type they are present for.
/// </remarks>
/// <param name="Type">The status type.</param>
/// <param name="LeaderId">The id of the leader, null if leaving. Present only for Leader type.</param>
/// <param name="LeaveType">The type of the leave type. Present only for Leave  type.</param>
/// <param name="ListMembersPlayerIds">The ids of players in the raid. Present only for ListMembers.</param>
/// <param name="PlayerHealths">Health of the players. Present only for PlayerHealths.</param>
[PacketHeader("raidf", PacketSource.Server)]
[PacketHeader("raid", PacketSource.Server)]
[GenerateSerializer(true)]
public record RaidPacket
(
    [PacketIndex(0)]
    RaidPacketType Type,
    [PacketConditionalIndex(1, "Type", false, RaidPacketType.Leader, IsOptional = true)]
    long? LeaderId,
    [PacketConditionalIndex(2, "Type", false, RaidPacketType.Leave, IsOptional = true)]
    RaidLeaveType? LeaveType,
    [PacketConditionalListIndex(3, "Type", false, RaidPacketType.ListMembers, ListSeparator = ' ', IsOptional = true)]
    IReadOnlyList<long>? ListMembersPlayerIds,
    [PacketConditionalListIndex(4, "Type", false, RaidPacketType.PlayerHealths, InnerSeparator = '.', ListSeparator = ' ', IsOptional = true)]
    IReadOnlyList<RaidPlayerHealthsSubPacket>? PlayerHealths
) : IPacket;