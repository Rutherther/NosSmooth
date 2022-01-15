//
//  FcSubPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Act4;

/// <summary>
/// Sub packet of <see cref="FcPacket"/>, containing
/// either the state of angels or demons
/// </summary>
/// <param name="Percentage">The percentage of the faction.</param>
/// <param name="Mode">The mode </param>
/// <param name="CurrentTime">The time the raid has been open for so far.</param>
/// <param name="TotalTime">The total time the raid will be open for.</param>
/// <param name="IsMorcos">Whether the current raid is Marcos.</param>
/// <param name="IsHatus">Whether the current raid is Hatus.</param>
/// <param name="IsCalvina">Whether the current raid is Calvina.</param>
/// <param name="IsBerios">Whether the current raid is Berios.</param>
/// <param name="Unknown">Unknown value, seems to be always 0.</param>
[GenerateSerializer(true)]
[PacketHeader(null, PacketSource.Server)]
public record FcSubPacket
(
    [PacketIndex(0)]
    short Percentage,
    [PacketIndex(1)]
    Act4Mode Mode,
    [PacketIndex(2)]
    long CurrentTime,
    [PacketIndex(3)]
    long TotalTime,
    [PacketIndex(4)]
    bool IsMorcos,
    [PacketIndex(5)]
    bool IsHatus,
    [PacketIndex(6)]
    bool IsCalvina,
    [PacketIndex(7)]
    bool IsBerios,
    [PacketIndex(8)]
    byte Unknown
) : IPacket;