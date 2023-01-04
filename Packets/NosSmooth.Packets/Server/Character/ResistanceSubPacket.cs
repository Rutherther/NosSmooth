//
//  ResistanceSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// A sub packet containing information about resistance.
/// </summary>
/// <param name="FireResistance">The fire resistance.</param>
/// <param name="WaterResistance">The water resistance.</param>
/// <param name="LightResistance">The light resistance.</param>
/// <param name="DarkResistance">The dark resistance.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record ResistanceSubPacket
(
    [PacketIndex(0)]
    short FireResistance,
    [PacketIndex(1)]
    short WaterResistance,
    [PacketIndex(2)]
    short LightResistance,
    [PacketIndex(3)]
    short DarkResistance
) : IPacket;