//
//  PartnerSkillRank.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Mates;

/// <summary>
/// Ranks of partner sp skills.
/// </summary>
public enum PartnerSkillRank
{
    /// <summary>
    /// The skills rank must be revealed first.
    /// </summary>
    NotIdentifiedYet = 0,

    /// <summary>
    /// Worst skill rank.
    /// </summary>
    F = 1,

    /// <summary>
    /// E skill rank.
    /// </summary>
    E = 2,

    /// <summary>
    /// D skill rank.
    /// </summary>
    D = 3,

    /// <summary>
    /// C skill rank.
    /// </summary>
    C = 4,

    /// <summary>
    /// B skill rank.
    /// </summary>
    B = 5,

    /// <summary>
    /// A skill rank.
    /// </summary>
    A = 6,

    /// <summary>
    /// Best skill rank.
    /// </summary>
    S = 7
}