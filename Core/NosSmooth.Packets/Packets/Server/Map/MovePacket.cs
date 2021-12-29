//
//  MovePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Shared.Enumerations;
using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Map;

/// <summary>
/// The entity has moved to the given position.
/// </summary>
/// <param name="EntityType"></param>
/// <param name="EntityId"></param>
/// <param name="MapX"></param>
/// <param name="MapY"></param>
/// <param name="Speed"></param>
[PacketHeader("mv", PacketSource.Server)]
[GenerateSerializer]
public record MovePacket
(
    [PacketIndex(0)]
    VisualType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    short MapX,
    [PacketIndex(3)]
    short MapY,
    [PacketIndex(4)]
    byte Speed
) : IPacket;