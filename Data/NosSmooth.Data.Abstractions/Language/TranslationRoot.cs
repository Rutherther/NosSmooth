//
//  TranslationRoot.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Language;

/// <summary>
/// Root type of a translation.
/// </summary>
public enum TranslationRoot
{
    /// <summary>
    /// The translation is for a card.
    /// </summary>
    Card,

    /// <summary>
    /// The translation is for an item.
    /// </summary>
    Item,

    /// <summary>
    /// The translation is for a monster.
    /// </summary>
    Monster,

    /// <summary>
    /// The translation for a skill.
    /// </summary>
    Skill,

    /// <summary>
    /// The translation for a map.
    /// </summary>
    Map
}