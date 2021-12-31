//
//  FamilySubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Players;

/// <summary>
/// Sub packet with family information.
/// </summary>
/// <param name="FamilyId">The id of the family.</param>
/// <param name="Title">The title of the family.</param>
/// <param name="FamilyName">The name of the family.</param>
[GenerateSerializer]
public record FamilySubPacket
(
    [PacketIndex(0, AfterSeparator = '.')]
    string FamilyId,
    [PacketIndex(1)]
    short Title,
    [PacketIndex(2)]
    string FamilyName
) : IPacket;