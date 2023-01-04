//
//  Bgm2Packet.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// A music to be played.
/// </summary>
/// <param name="MusicId">The id of the music to be played.</param>
[PacketHeader("bgm2", PacketSource.Server)]
[GenerateSerializer(true)]
public record Bgm2Packet
(
    [PacketIndex(0)]
    long MusicId
) : IPacket;