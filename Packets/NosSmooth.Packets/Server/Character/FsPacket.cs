//
//  FsPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Character's faction he belongs to.
/// </summary>
/// <param name="Faction">The faction the character belongs to.</param>
[PacketHeader("fs", PacketSource.Server)]
[GenerateSerializer(true)]
public record FsPacket
(
    [PacketIndex(0)]
    FactionType Faction
) : IPacket;