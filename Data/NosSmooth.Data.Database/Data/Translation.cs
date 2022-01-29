//
//  Translation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Database.Data;

/// <summary>
/// A translation of NosTale string.
/// </summary>
public class Translation
{
    /// <summary>
    /// Gets or sets the language of the translation.
    /// </summary>
    public Language Language { get; set; }

    /// <summary>
    /// Gets or sets the root key of the translation.
    /// </summary>
    public TranslationRoot Root { get; set; }

    /// <summary>
    /// Gets or sets the key of the translation.
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// Gets or sets the translation string.
    /// </summary>
    public string Translated { get; set; } = null!;
}