//
//  StatPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Entities;

/// <summary>
/// Sent when player takes damage, heals or changes options.
/// </summary>
/// <param name="Hp"></param>
/// <param name="HpMaximum"></param>
/// <param name="Mp"></param>
/// <param name="MpMaximum"></param>
/// <param name="Unknown">Unknown TODO.</param>
/// <param name="Option">Contains blockages (family block, exchange block etc). TODO add correct options.</param>
[PacketHeader("stat", PacketSource.Server)]
[GenerateSerializer(true)]
public record StatPacket
(
    [PacketIndex(0)]
    long Hp,
    [PacketIndex(1)]
    long HpMaximum,
    [PacketIndex(2)]
    long Mp,
    [PacketIndex(3)]
    long MpMaximum,
    [PacketIndex(4)]
    int Unknown,
    [PacketIndex(5)]
    long Option
) : IPacket;