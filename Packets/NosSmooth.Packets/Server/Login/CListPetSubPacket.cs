//
//  CListPetSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Login;

/// <summary>
/// An item of pet list inside of <see cref="CListPacket"/>.
/// </summary>
/// <param name="Skin">Skin of partner.</param>
/// <param name="PetVNum">The vnum of the pet.</param>
[PacketHeader(null, PacketSource.Server)]
[GenerateSerializer(true)]
public record CListPetSubPacket
(
    [PacketIndex(0)]
    long? Skin,
    [PacketIndex(1)]
    long? PetVNum
) : IPacket;