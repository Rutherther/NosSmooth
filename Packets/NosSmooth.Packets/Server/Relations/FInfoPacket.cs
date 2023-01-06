//
//  FInfoPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Relations;

/// <summary>
/// Information update of friend of a character.
/// </summary>
/// <param name="PlayerId">The id of the friend.</param>
/// <param name="IsConnected">Whether the friend is connected.</param>
/// <param name="Name">The name of the friend.</param>[PacketHeader("finfo", PacketSource.Server)]
[GenerateSerializer(true)]
public record FInfoPacket
(
    [PacketIndex(0)]
    long PlayerId,
    [PacketIndex(1)]
    bool IsConnected,
    [PacketIndex(2)]
    NameString Name
) : IPacket;