﻿//
//  RaidJoinLeaveType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Raids;

namespace NosSmooth.Packets.Enums.Raids;

/// <summary>
/// A sub type of <see cref="RaidPacket"/>
/// in case the type of the packet is Leave.
/// </summary>
public enum RaidJoinLeaveType
{
    /// <summary>
    /// The player has left the raid.
    /// </summary>
    PlayerLeft = 0,

    /// <summary>
    /// The player has joined the raid.
    /// </summary>
    PlayerJoined = 1
}