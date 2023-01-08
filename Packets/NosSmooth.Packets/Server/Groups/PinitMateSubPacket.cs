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

/// <summary>
/// A sub packet of <see cref="PinitPacket"/>
/// representing a mate.
/// </summary>
/// <param name="MateType">The type of the mate.</param>
/// <param name="Level">The level of the mate.</param>
/// <param name="Name">The name of the mate.</param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="VNum">The VNum of the mate entity.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="Unknown2">Unknown TODO.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
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
    long? VNum,
    [PacketIndex(5)]
    byte Unknown1,
    [PacketIndex(6)]
    byte Unknown2
) : IPacket;