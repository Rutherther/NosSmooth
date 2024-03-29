﻿//
//  CModePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO.IsolatedStorage;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// TODO
/// </summary>
/// <param name="EntityType">The type of the entity the packet is for.</param>
/// <param name="EntityId">The id of the entity.</param>
/// <param name="MorphVNum">The morph vnum (used for special cards and such).</param>
/// <param name="MorphUpgrade">The upgrade of the morph. (wings)</param>
/// <param name="MorphDesign">The design of the wings.</param>
/// <param name="MorphBonus">Unknown TODO</param>
/// <param name="Size">The size of the entity on the screen.</param>
/// <param name="MorphSkin">Unknown TODO</param>
[PacketHeader("c_mode", PacketSource.Server)]
[GenerateSerializer(true)]
public record CModePacket
(
    [PacketIndex(0)]
    EntityType EntityType,
    [PacketIndex(1)]
    long EntityId,
    [PacketIndex(2, IsOptional = true)]
    long? MorphVNum,
    [PacketIndex(3, IsOptional = true)]
    byte? MorphUpgrade,
    [PacketIndex(4, IsOptional = true)]
    short? MorphDesign,
    [PacketIndex(5, IsOptional = true)]
    byte? MorphBonus,
    [PacketIndex(6, IsOptional = true)]
    byte? Size,
    [PacketIndex(7, IsOptional = true)]
    short? MorphSkin
) : IPacket;