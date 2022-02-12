//
//  MltObjPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Miniland;

/// <summary>
/// Miniland objects list packet.
/// </summary>
/// <remarks>
/// Used for minilands of different owners.
/// </remarks>
/// <param name="Objects">The miniland objects.</param>
[PacketHeader("mltobj", PacketSource.Server)]
[GenerateSerializer(true)]
public record MltObjPacket
(
    [PacketIndex(0)]
    IReadOnlyList<MltObjSubPacket> Objects
) : IPacket;