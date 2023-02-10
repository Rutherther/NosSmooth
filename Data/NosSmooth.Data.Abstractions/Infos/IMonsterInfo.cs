//
//  IMonsterInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Abstractions.Infos;

/// <summary>
/// The NosTale monster information.
/// </summary>
public interface IMonsterInfo : IVNumInfo
{
    /// <summary>
    /// Gets the name of the monster.
    /// </summary>
    TranslatableString Name { get; }

    /// <summary>
    /// Gets the default level of the monster.
    /// </summary>
    ushort Level { get; }

    /// <summary>
    /// Gets the default Range of the monster.
    /// </summary>
    short Range { get; }

    /// <summary>
    /// Gets the default NoticeRange of the monster.
    /// </summary>
    short NoticeRange { get; }

    /// <summary>
    /// Gets the default castTime of the monster.
    /// </summary>
    int CastTime { get; }

    /// <summary>
    /// Gets if the monster is Hostile.
    /// </summary>
    bool Hostile { get; }
}