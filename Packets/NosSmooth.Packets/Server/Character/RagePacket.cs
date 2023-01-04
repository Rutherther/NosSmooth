//
//  RagePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Current rage of the character.
/// </summary>
/// <param name="RagePoints">The rage points.</param>
/// <param name="RagePointsMax">The maximum amount of rage points.</param>
[PacketHeader("rage", PacketSource.Server)]
[GenerateSerializer(true)]
public record RagePacket
(
    [PacketIndex(0)]
    long RagePoints,
    [PacketIndex(1)]
    long RagePointsMax
) : IPacket;