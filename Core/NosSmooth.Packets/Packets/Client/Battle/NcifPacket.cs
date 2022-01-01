//
//  NcifPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Packets.Packets.Client.Battle;

/// <summary>
/// Client sends this when focusing an entity and then every x seconds again.
/// </summary>
/// <remarks>
/// The server should respond with "st" packet.
/// </remarks>
/// <param name="EntityType">The type of the focused entity.</param>
/// <param name="TargetId">The focused entity id.</param>
[PacketHeader("ncif", PacketSource.Client)]
[GenerateSerializer]
public record NcifPacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long TargetId
) : IPacket;