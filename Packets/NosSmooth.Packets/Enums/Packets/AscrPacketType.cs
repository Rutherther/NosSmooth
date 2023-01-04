//
//  AscrPacketType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Character;

namespace NosSmooth.Packets.Enums.Packets;

/// <summary>
/// Type of <see cref="AscrPacket"/>.
/// </summary>
public enum AscrPacketType
{
    /// <summary>
    /// Unknown TODO.
    /// </summary>
    Close = -1,

    /// <summary>
    /// Unknown TOOD.
    /// </summary>
    Alone = 0,

    /// <summary>
    /// Unknown TODO.
    /// </summary>
    Group = 1,

    /// <summary>
    /// Unknown TODO.
    /// </summary>
    Family = 2
}