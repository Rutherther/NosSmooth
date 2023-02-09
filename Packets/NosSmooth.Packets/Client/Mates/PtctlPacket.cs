//
//  PtctlPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Client.Mates;

/// <summary>
/// A mate move packet, moving multiple mates at a time.
/// </summary>
/// <param name="MapId">The current map id.</param>
/// <param name="ControlsCount">The count of controls in array.</param>
/// <param name="Controls">The array containing the mates to move and positions to move them to.</param>
/// <param name="Unknown">Seems to always be euqal to first EntityId in Controls.</param>
/// <param name="Unknown1">Seems to always be 9.</param>
[PacketHeader("ptctl", PacketSource.Server)]
[GenerateSerializer(true)]
public record PtctlPacket
(
    [PacketIndex(0)]
    short MapId,
    [PacketIndex(1)]
    uint? ControlsCount,
    [PacketContextList(2, "ControlsCount", ListSeparator = ' ', InnerSeparator = ' ')]
    IReadOnlyList<PtctlSubPacket> Controls,
    [PacketIndex(3)]
    long Unknown,
    [PacketIndex(4)]
    short Unknown1
) : IPacket;