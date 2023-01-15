//
//  VbPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Buffs;

/// <summary>
/// A static buff effect.
/// </summary>
/// <param name="CardId">The buff card id.</param>
/// <param name="IsActive">Whether the buff is active.</param>
/// <param name="Time">The duration of the buff.</param>
[PacketHeader("vb", PacketSource.Server)]
[GenerateSerializer(true)]
public record VbPacket
(
    [PacketIndex(0)]
    short CardId,
    [PacketIndex(1)]
    bool IsActive,
    [PacketIndex(2)]
    int? Time
) : IPacket;