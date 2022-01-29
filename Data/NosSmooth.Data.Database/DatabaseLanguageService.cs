//
//  DatabaseLanguageService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NosSmooth.Data.Abstractions.Language;
using Remora.Results;

namespace NosSmooth.Data.Database;

/// <inheritdoc />
public class DatabaseLanguageService : ILanguageService
{
    private readonly IDbContextFactory<NostaleDataContext> _dbContextFactory;
    private readonly LanguageServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseLanguageService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="options">The options.</param>
    public DatabaseLanguageService
    (
        IDbContextFactory<NostaleDataContext> dbContextFactory,
        IOptions<LanguageServiceOptions> options
    )
    {
        CurrentLanguage = options.Value.Language;
        _dbContextFactory = dbContextFactory;
        _options = options.Value;
    }

    /// <inheritdoc />
    public Language CurrentLanguage { get; set; }

    /// <inheritdoc />
    public async Task<Result<string>> GetTranslationAsync(TranslationRoot root, string key, Language? language = default, CancellationToken ct = default)
    {
        try
        {
            language ??= CurrentLanguage;
            await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
            var translation = await context.Translations.FirstOrDefaultAsync
                (x => x.Root == root && x.Key == key && x.Language == language, ct);
            if (translation is null)
            {
                return new NotFoundError($"Could not find translation for {root} {key}");
            }

            return translation.Translated;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public async Task<Result<string>> GetTranslationAsync
        (TranslatableString translatableString, Language? language = default, CancellationToken ct = default)
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