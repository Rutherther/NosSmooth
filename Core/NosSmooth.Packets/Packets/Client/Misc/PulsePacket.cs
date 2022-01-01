//
//  PulsePacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Client.Misc;

/// <summary>
/// Heartbeat packet.
/// </summary>
/// <remarks>
/// Sent every minute the client is connected on the server.
/// </remarks>
/// <param name="Seconds">The number of seconds till connected. (should be sent only as multiples of 60)</param>
/// <param name="Unknown">Unknown TODO</param>
[GenerateSerializer]
[PacketHeader("pulse", PacketSource.Client)]
public record PulsePacket
(
    [PacketIndex(0)]
    long Seconds,
    [PacketIndex(1)]
    short Unknown
);