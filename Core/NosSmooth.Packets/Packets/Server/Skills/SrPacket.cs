//
//  SrPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Skills;

/// <summary>
/// Skill cooldown reset.
/// </summary>
/// <remarks>
/// The client will mark the skill as ready only after this packet is received.
/// Do not rely on skill used + cooldown time. You will get kicked out of the sever
/// after some time (on official).
/// </remarks>
/// <param name="SkillId">The id of the skill that is resetted.</param>
[PacketHeader("sr", PacketSource.Server)]
[GenerateSerializer(true)]
public record SrPacket
(
    [PacketIndex(0)]
    long SkillId
) : IPacket;