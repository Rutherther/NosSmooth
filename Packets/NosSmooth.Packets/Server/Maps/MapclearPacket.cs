//
//  MapclearPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// Entities on the map were cleared.
/// </summary>
[PacketHeader("mapclear", PacketSource.Server)]
[GenerateSerializer(true)]
public record MapclearPacket() : IPacket;