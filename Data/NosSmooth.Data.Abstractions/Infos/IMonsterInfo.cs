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
}