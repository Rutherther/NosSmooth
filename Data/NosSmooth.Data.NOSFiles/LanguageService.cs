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
    private readonly
        IReadOnlyDictionary<Language, IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>
        _translations;

    private readonly LanguageServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageService"/> class.
    /// </summary>
    /// <param name="translations">The translations.</param>
    /// <param name="options">The options.</param>
    public LanguageService
    (
        IReadOnlyDictionary<Language, IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>
            translations,
        LanguageServiceOptions options
    )
    {
        CurrentLanguage = options.Language;
        _translations = translations;
        _options = options;
    }

    /// <inheritdoc/>
    public Language CurrentLanguage { get; set; }

    /// <inheritdoc/>
    public Task<Result<string>> GetTranslationAsync
    (
        TranslationRoot root,
        string key,
        Language? language = default,
        CancellationToken ct = default
    )
    {
        if (!_translations.ContainsKey(language ?? CurrentLanguage))
        {
            return Task.FromResult
            (
                Result<string>.FromError
                    (new NotFoundError($"The requested language {language ?? CurrentLanguage} is not parsed."))
            );
        }

        var translations = _translations[language ?? CurrentLanguage];
        if (!translations.ContainsKey(root))
        {
            return Task.FromResult(Result<string>.FromSuccess(key));
        }

        var keyTranslations = translations[root];
        if (!keyTranslations.ContainsKey(key))
        {
            return Task.FromResult(Result<string>.FromSuccess(key));
        }

        return Task.FromResult(Result<string>.FromSuccess(keyTranslations[key]));
    }

    /// <inheritdoc/>
    public async Task<Result<string>> GetTranslationAsync
    (
        TranslatableString translatableString,
        Language? language = default,
        CancellationToken ct = default
    )
    {
        var translation = await GetTranslationAsync(translatableString.Root, translatableString.Key, language, ct);
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