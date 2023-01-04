//
//  CInfoResetPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// Reset information about the character
/// (except for stuff from c_info)
/// </summary>
/// <remarks>
/// Sent upon game initialization,
/// seems that it should reset the
/// character information. May be
/// useful when relogging as
/// different character.
///
/// This is sent AFTER c_info,
/// that means not everything from
/// the character should be reset,
/// only fields that are not
/// in c_info.
/// </remarks>
[PacketHeader("c_info_reset", PacketSource.Server)]
[GenerateSerializer(true)]
public record CInfoResetPacket() : IPacket;