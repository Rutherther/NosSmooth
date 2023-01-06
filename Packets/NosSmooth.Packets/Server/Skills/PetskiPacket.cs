//
//  PetskiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Skills;

/// <summary>
/// Information about pet skills.
/// </summary>
/// <param name="SkillVNum">The vnum of the pet skill.</param>
[PacketHeader("petski", PacketSource.Server)]
[GenerateSerializer(true)]
public record PetskiPacket
(
    [PacketIndex(0)]
    int? SkillVNum
) : IPacket;