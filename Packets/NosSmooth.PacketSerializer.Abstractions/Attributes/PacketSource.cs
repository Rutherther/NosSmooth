//
//  PacketSource.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializer.Abstractions.Attributes;

/// <summary>
/// Specifies the source of the packet.
/// </summary>
public enum PacketSource
{
    /// <summary>
    /// The packet is from the server.
    /// </summary>
    Server,

    /// <summary>
    /// The packet is from the client.
    /// </summary>
    Client
}