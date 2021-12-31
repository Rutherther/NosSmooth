//
//  Element.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums;

/// <summary>
/// Element type.
/// </summary>
public enum Element
{
    /// <summary>
    /// No element type.
    /// </summary>
    Neutral,

    /// <summary>
    /// Fire element.
    /// </summary>
    Fire,

    /// <summary>
    /// Water element.
    /// </summary>
    Water,

    /// <summary>
    /// Light element.
    /// </summary>
    Light,

    /// <summary>
    /// Dark element.
    /// </summary>
    Dark
}