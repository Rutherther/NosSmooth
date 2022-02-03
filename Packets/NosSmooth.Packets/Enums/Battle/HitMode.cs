//
//  HitMode.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Server.Battle;

namespace NosSmooth.Packets.Enums.Battle;

/// <summary>
/// Hit mode used in <see cref="SuPacket"/>.
/// </summary>
public enum HitMode
{
    /// <summary>
    /// The attack has hit successfully.
    /// </summary>
    SuccessfulAttack = 0,

    /// <summary>
    /// The attack ended in a miss.
    /// </summary>
    /// <remarks>
    /// No dealt damage.
    /// </remarks>
    Miss = 1,

    /// <summary>
    /// The attack has hit successfully and it was a critical hit.
    /// </summary>
    /// <remarks>
    /// The damage dealt is higher.
    /// </remarks>
    CriticalAttack = 3,

    /// <summary>
    /// The attack ended in a miss.
    /// </summary>
    LongRangeMiss = 4,

    /// <summary>
    /// The buff was successfully .
    /// </summary>
    SuccessfulBuff = 5, // probably successful buff

    /// <summary>
    /// Unknown TODO.
    /// </summary>
    Unknown = -2,
}