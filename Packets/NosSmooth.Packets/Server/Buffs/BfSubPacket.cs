//
//  BfSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Buffs;

/// <summary>
/// A sub packet of <see cref="BfPacket"/>.
/// </summary>
/// <param name="Charge">Uknown TODO.</param>
/// <param name="CardId">The buff card id.</param>
/// <param name="Time">The duration of the buff.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record BfSubPacket
(
    [PacketIndex(0)]
    int Charge,
    [PacketIndex(1)]
    short CardId,
    [PacketIndex(2)]
    int Time
) : IPacket;