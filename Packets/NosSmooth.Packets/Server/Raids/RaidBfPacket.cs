//
//  RaidBfPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Raids;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Raids;

/// <summary>
/// Raid ui packet. Unknown function.
/// </summary>
/// <param name="Type">Unknown TODO.</param>
/// <param name="WindowType">Unknown TODO.</param>
/// <param name="Unknown">Unknown TODO.</param>
[PacketHeader("raidbf", PacketSource.Server)]
[GenerateSerializer(true)]
public record RaidBfPacket
(
    [PacketIndex(0)]
    byte Type,
    [PacketIndex(1)]
    RaidBfPacketType WindowType,
    [PacketIndex(2)]
    byte Unknown
) : IPacket;