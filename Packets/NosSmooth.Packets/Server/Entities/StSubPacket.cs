//
//  StSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Entities;

/// <summary>
/// A sub packet for <see cref="StPacket"/> representing buff card id and level.
/// </summary>
/// <param name="CardId">The buff card id.</param>
/// <param name="Level">The buff level.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record StSubPacket
(
    [PacketIndex(0)]
    short CardId,
    [PacketIndex(1, IsOptional = true)]
    short? Level
) : IPacket;