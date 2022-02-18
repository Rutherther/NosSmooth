//
//  CancelPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Skills;

/// <summary>
/// Packet that cancels an operation like a skill.
/// </summary>
/// <param name="Type"></param>
/// <param name="TargetId"></param>
/// <param name="Unknown"></param>
[PacketHeader("cancel", PacketSource.Server)]
[GenerateSerializer(true)]
public record CancelPacket
(
    [PacketIndex(0)]
    short Type,
    [PacketIndex(1)]
    long TargetId,
    [PacketIndex(2)]
    long Unknown = -1
) : IPacket;