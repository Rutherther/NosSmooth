//
//  RaidPacketType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Raids;

namespace NosSmooth.Packets.Enums.Raids;

/// <summary>
/// A type of <see cref="RaidPacket"/>.
/// </summary>
public enum RaidPacketType
{
    /// <summary>
    /// A list of member ids follows.
    /// </summary>
    ListMembers = 0,

    /// <summary>
    /// Character left or the raid is finished.
    /// </summary>
    JoinLeave = 1,

    /// <summary>
    /// Leader id follows (or -1 in case of leave).
    /// </summary>
    Leader = 2,

    /// <summary>
    /// Hp, mp stats of players follow.
    /// </summary>
    PlayerHealths = 3,

    /// <summary>
    /// Sent after raid start, but before refresh members.
    /// </summary>
    AfterStartBeforeRefreshMembers = 4,

    /// <summary>
    /// Raid has just started.
    /// </summary>
    Start = 5
}