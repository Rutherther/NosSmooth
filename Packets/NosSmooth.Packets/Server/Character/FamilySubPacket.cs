//
//  FamilySubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Sub packet with family information.
/// </summary>
/// <param name="FamilyId">The id of the family.</param>
/// <param name="Title">The title of the family.</param>
[GenerateSerializer(true)]
[PacketHeader(null, PacketSource.Server)]
public record FamilySubPacket
(
    [PacketIndex(0)]
    long FamilyId,
    [PacketIndex(1)]
    short Title
) : IPacket;