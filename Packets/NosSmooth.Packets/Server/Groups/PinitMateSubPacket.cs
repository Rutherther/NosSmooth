//
//  PinitMateSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.Mates;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Groups;

public record PinitMateSubPacket
(
    [PacketIndex(0)]
    MateType MateType,
    [PacketIndex(1)]
    byte Level,
    [PacketIndex(2)]
    NameString? Name,
    [PacketIndex(3)]
    int? Unknown,
    [PacketIndex(4)]
    long? VNum
) : IPacket;