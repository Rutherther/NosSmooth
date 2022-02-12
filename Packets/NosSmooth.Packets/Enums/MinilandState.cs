//
//  MinilandState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums;

/// <summary>
/// State of a miniland.
/// </summary>
public enum MinilandState
{
    /// <summary>
    /// The miniland is open for anybody.
    /// </summary>
    Open,

    /// <summary>
    /// The miniland is closed, cannot be accessed by anyone.
    /// </summary>
    Private,

    /// <summary>
    /// The miniland is locked, cannot be accessed and objects can be built.
    /// </summary>
    Lock,
}