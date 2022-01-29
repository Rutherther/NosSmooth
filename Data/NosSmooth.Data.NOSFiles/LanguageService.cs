//
//  LanguageService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;
using NosSmooth.Data.Abstractions.Language;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles;

/// <inheritdoc />
internal class LanguageService : ILanguageService
{
    private readonly IReadOnlyDictionary<Language, IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>> _translations;
    private readonly LanguageServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService"/> class.
    /// </summary>
    /// <param name="translations">The translations.</param>
    /// <param name="options">The options.</param>
    public LanguageService(IReadOnlyDictionary<Language, IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>> translations, LanguageServiceOptions options)
    {
        CurrentLanguage = options.Language;
        _translations = translations;
        _options = options;
    }

    /// <inheritdoc/>
    public Language CurrentLanguage { get; set; }

    /// <inheritdoc/>
    public Result<string> GetTranslation(TranslationRoot root, string key, Language? language = default)
    {
        if (!_translations.ContainsKey(language ?? CurrentLanguage))
        {
            return new NotFoundError($"The requested language {language ?? CurrentLanguage} is not parsed.");
        }

        var translations = _translations[language ?? CurrentLanguage];
        if (!translations.ContainsKey(root))
        {
            return key;
        }

        var keyTranslations = translations[root];
        if (!keyTranslations.ContainsKey(key))
        {
            return key;
        }

        return keyTranslations[key];
    }

    /// <inheritdoc/>
    public Result<string> GetTranslation(TranslatableString translatableString, Language? language = default)
    {
        var translation = GetTranslation(translatableString.Root, translatableString.Key, language);
        if (!translation.IsSuccess)
        {
            return translation;
        }

        if (_options.FillTranslatableStrings)
        {
            translatableString.Fill(translation.Entity);
        }

        return translation;
    }
}