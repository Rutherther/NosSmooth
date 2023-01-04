//
//  GoldPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Inventory;

/// <summary>
/// Amount of gold the character has.
/// </summary>
/// <param name="Gold">The amount of gold the character has.</param>
/// <param name="Unknown">Unknown TODO.</param>
[PacketHeader("gold", PacketSource.Server)]
[GenerateSerializer(true)]
public record GoldPacket
(
    [PacketIndex(0)]
    long Gold,
    [PacketIndex(1)]
    byte Unknown
) : IPacket;