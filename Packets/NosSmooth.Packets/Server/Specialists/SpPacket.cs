//
//  SpPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Specialists;

/// <summary>
/// Packet with information about sp points.
/// </summary>
/// <remarks>
/// Sent on login, on sp change, on points change.
/// </remarks>
/// <param name="AdditionalSpPoints">The additional sp points used after sp points are 0.</param>
/// <param name="MaxAdditionalSpPoints">The maximum of additional sp points.</param>
/// <param name="SpPoints">The sp points that decrease upon using sp.</param>
/// <param name="MaxSpPoints">The maximum of sp points.</param>
[PacketHeader("sp", PacketSource.Server)]
[GenerateSerializer(true)]
public record SpPacket
(
    [PacketIndex(0)]
    int AdditionalSpPoints,
    [PacketIndex(1)]
    int MaxAdditionalSpPoints,
    [PacketIndex(2)]
    int SpPoints,
    [PacketIndex(3)]
    int MaxSpPoints
) : IPacket;