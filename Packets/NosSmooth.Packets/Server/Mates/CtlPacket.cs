//
//  CtlPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Mates;

/// <summary>
/// The client may control the given pet now.
/// </summary>
/// <param name="EntityType">The type of the mate entity (npc).</param>
/// <param name="EntityId">The id of the mate entity.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="Unknown2">Unknown TODO.</param>
[PacketHeader("ctl", PacketSource.Server)]
[GenerateSerializer(true)]
public record CtlPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2)]
    byte Unknown1 = 3,
    [PacketIndex(3)]
    byte Unknown2 = 0
) : IPacket;