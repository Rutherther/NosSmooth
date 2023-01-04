//
//  TitleSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// A sub packet for <see cref="TitlePacket"/>
/// with information about and obtained title.
/// </summary>
/// <param name="TitleId">The id of the title.</param>
/// <param name="TitleStatus">The status of the title.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record TitleSubPacket
(
    [PacketIndex(0)]
    short TitleId,
    [PacketIndex(1)]
    byte TitleStatus
) : IPacket;