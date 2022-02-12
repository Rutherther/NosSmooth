//
//  SdPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Specialists;

/// <summary>
/// Packet for sp cooldown.
/// </summary>
/// <remarks>
/// Doesn't block putting on the sp. Just shows loading on the character icon.
/// </remarks>
/// <param name="Cooldown">The cooldown.</param>
[PacketHeader("sd", PacketSource.Server)]
[GenerateSerializer(true)]
public record SdPacket
(
    [PacketIndex(0)]
    short Cooldown
) : IPacket;