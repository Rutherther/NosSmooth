//
//  Element.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Enums;

/// <summary>
/// Element type.
/// </summary>
public enum Element
{
    /// <summary>
    /// No element type.
    /// </summary>
    Neutral = 0,

    /// <summary>
    /// Fire element.
    /// </summary>
    Fire = 1,

    /// <summary>
    /// Water element.
    /// </summary>
    Water = 2,

    /// <summary>
    /// Light element.
    /// </summary>
    Light = 3,

    /// <summary>
    /// Dark element.
    /// </summary>
    Dark = 4
}