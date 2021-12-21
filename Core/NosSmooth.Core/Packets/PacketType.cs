//
//  PacketType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Packets;

/// <summary>
/// The type of the packet.
/// </summary>
public enum PacketType
{
    /// <summary>
    /// The packet was sent to the server.
    /// </summary>
    Sent,

    /// <summary>
    /// The packet was received from the server.
    /// </summary>
    Received,
}