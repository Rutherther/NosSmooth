//
//  Act4Mode.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums;

/// <summary>
/// The act4 mode.
/// </summary>
public enum Act4Mode
{
    /// <summary>
    /// There is nothing currently running, percentage has to reach 100 % for Mukraju to spawn.
    /// </summary>
    None = 0,

    /// <summary>
    /// Mukraju has been spawned and has to be killed in order to start raid.
    /// </summary>
    Mukraju = 1,

    /// <summary>
    /// Unknown mode.
    /// </summary>
    Unknown = 2,

    /// <summary>
    /// Raid is in progress.
    /// </summary>
    Raid = 3,
}