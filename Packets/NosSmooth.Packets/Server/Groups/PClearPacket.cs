//
//  PClearPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Groups;

/// <summary>
/// Clear the characters's group.
/// </summary>
[PacketHeader("p_clear", PacketSource.Server)]
[GenerateSerializer(true)]
public record PClearPacket() : IPacket;