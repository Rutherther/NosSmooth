//
//  MlObjLstPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Miniland;

/// <summary>
/// Miniland object list packet.
/// </summary>
/// <param name="Objects">The objects in the miniland or inventory.</param>
[PacketHeader("mlobjlst", PacketSource.Server)]
[GenerateSerializer(true)]
public record MlObjLstPacket
(
    [PacketListIndex(0, ListSeparator = ' ', InnerSeparator = '.')]
    IReadOnlyList<MlObjPacket> Objects
) : IPacket;