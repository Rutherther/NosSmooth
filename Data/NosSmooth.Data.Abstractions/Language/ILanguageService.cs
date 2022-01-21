//
//  ILanguageService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Data.Abstractions.Language;

/// <summary>
/// Service for translating NosTale strings.
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Gets or sets the current language.
    /// </summary>
    public Language CurrentLanguage { get; set; }

    /// <summary>
    /// Gets the translation of the given key.
    /// </summary>
    /// <param name="root">The root type of the key.</param>
    /// <param name="key">The key to translate.</param>
    /// <param name="language">The language, <see cref="CurrentLanguage"/> will be used if null.</param>
    /// <returns>The translated string or an error.</returns>
    public Result<string> GetTranslation(TranslationRoot root, LanguageKey key, Language? language = default);
}