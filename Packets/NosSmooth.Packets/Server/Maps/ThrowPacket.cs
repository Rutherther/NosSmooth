//
//  ThrowPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Maps;

/// <summary>
/// An item has been thrown from the given position to target position.
/// </summary>
/// <remarks>
/// Used in raids after boss is killed.
/// </remarks>
/// <param name="ItemVNum">The vnum of the dropped item.</param>
/// <param name="DropId">The id of the item.</param>
/// <param name="SourceX">The source x coordinate the item is thrown from.</param>
/// <param name="SourceY">The source y coordinate the item is thrown from.</param>
/// <param name="TargetX">The target x coordinate the item is thrown to.</param>
/// <param name="TargetY">The target y coordinate the item is thrown to.</param>
/// <param name="Amount">The amount of items thrown.</param>
[PacketHeader("throw", PacketSource.Server)]
[GenerateSerializer(true)]
public record ThrowPacket
(
    [PacketIndex(0)]
    int ItemVNum,
    [PacketIndex(1)]
    long DropId,
    [PacketIndex(2)]
    short SourceX,
    [PacketIndex(3)]
    short SourceY,
    [PacketIndex(4)]
    short TargetX,
    [PacketIndex(5)]
    short TargetY,
    [PacketIndex(6)]
    int Amount
) : IPacket;