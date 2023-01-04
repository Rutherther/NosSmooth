//
//  QSlotType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Packets;

/// <summary>
/// Type of a qslot packet.
/// </summary>
public enum QSlotType
{
    /// <summary>
    /// Unknown TODO.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Set the given slot to new value.
    /// </summary>
    Set = 1,

    /// <summary>
    /// Move the given slot item.
    /// </summary>
    Move = 2,

    /// <summary>
    /// Remove the given slot item.
    /// </summary>
    Remove = 3,

    /// <summary>
    /// Reset the given slot.
    /// </summary>
    Reset = 7
}