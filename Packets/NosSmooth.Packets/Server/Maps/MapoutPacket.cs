//
//  MapoutPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// Teleported out of map.
/// </summary>
[PacketHeader("mapout", PacketSource.Server)]
[GenerateSerializer(true)]
public record MapoutPacket() : IPacket;