//
//  TitlePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Titles the player has obtained.
/// </summary>
/// <param name="TitleSubPackets">The obtained titles.</param>
[PacketHeader("title", PacketSource.Server)]
[GenerateSerializer(true)]
public record TitlePacket
(
    [PacketListIndex(0, InnerSeparator = '.', ListSeparator = ' ')]
    IReadOnlyList<TitleSubPacket> TitleSubPackets
) : IPacket;