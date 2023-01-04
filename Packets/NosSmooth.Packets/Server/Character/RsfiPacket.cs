//
//  RsfiPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// A packet containing information about act that
/// the client should play story.
/// </summary>
/// <param name="Act">The act.</param>
/// <param name="ActPart">The part of the act.</param>
/// <param name="Unknown1">Unknown TODO.</param>
/// <param name="Unknown2">Unknown TODO.</param>
/// <param name="Ts">Unknown TODO.</param>
/// <param name="TsMax">Unknown TODO.</param>
[PacketHeader("rsfi", PacketSource.Server)]
[GenerateSerializer(true)]
public record RsfiPacket
(
    [PacketIndex(0)]
    byte Act,
    [PacketIndex(1)]
    byte ActPart,
    [PacketIndex(2)]
    byte Unknown1,
    [PacketIndex(3)]
    byte Unknown2,
    [PacketIndex(4)]
    byte Ts,
    [PacketIndex(5)]
    byte TsMax
) : IPacket;