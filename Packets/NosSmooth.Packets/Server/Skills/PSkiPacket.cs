//
//  PSkiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Skills;

/// <summary>
/// Partner skills.
/// </summary>
[PacketHeader("pski", PacketSource.Server)]
[GenerateSerializer(true)]
public record PSkiPacket
(
    [PacketListIndex(0, ListSeparator = ' ')]
    IReadOnlyList<int?> SkillVNums
) : IPacket;