//
//  TranslatableString.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.Data.Abstractions.Language;

/// <summary>
/// Represents a string that may be translated.
/// </summary>
/// <param name="Root">The root key of the translations.</param>
/// <param name="Key">The key of the string translation.</param>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Standard.")]
public record struct TranslatableString(TranslationRoot Root, string Key)
{
    /// <summary>
    /// Gets the translated string, if available.
    /// If not available, the key will be returned.
    /// </summary>
    public string Translated { get; private set; } = Key;

    /// <summary>
    /// Fill this translatable string with a translation.
    /// </summary>
    /// <param name="translation">The translation to fill.</param>
    public void Fill(string translation)
    {
        Translated = translation;
    }
}