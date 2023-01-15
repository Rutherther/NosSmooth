//
//  Act6Packet.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.UI;

/// <summary>
/// Unknown TODO.
/// </summary>
[PacketHeader("act6", PacketSource.Server)]
[GenerateSerializer(true)]
public record Act6Packet() : IPacket;