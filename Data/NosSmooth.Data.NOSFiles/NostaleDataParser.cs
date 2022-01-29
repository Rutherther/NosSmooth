//
//  NostaleDataParser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Files;
using NosSmooth.Data.NOSFiles.Options;
using NosSmooth.Data.NOSFiles.Parsers;
using NosSmooth.Data.NOSFiles.Readers;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles;

/// <summary>
/// Parser of NosTale .NOS files.
/// </summary>
public class NostaleDataParser
{
    private readonly FileReader _fileReader;
    private readonly NostaleDataOptions _options;
    private readonly LanguageServiceOptions _languageOptions;
    private NostaleData? _parsed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleDataParser"/> class.
    /// </summary>
    /// <param name="fileReader">The file reader.</param>
    /// <param name="options">The options.</param>
    /// <param name="languageOptions">The language options.</param>
    public NostaleDataParser
    (
        FileReader fileReader,
        IOptions<NostaleDataOptions> options,
        IOptions<LanguageServiceOptions> languageOptions
    )
    {
        _fileReader = fileReader;
        _options = options.Value;
        _languageOptions = languageOptions.Value;
    }

    /// <summary>
    /// Extract NosTale files from archives.
    /// </summary>
    /// <param name="path">The path to the nostale data files.</param>
    /// <param name="languages">The languages to include.</param>
    /// <returns>The nostale files.</returns>
    public Result<NostaleFiles> GetFiles(string? path = null, params Language[] languages)
    {
        string datFilesPath = Path.Combine(path ?? _options.NostaleDataPath, _options.InfosFileName);
        string mapGridsFilesPath = Path.Combine(path ?? _options.NostaleDataPath, _options.MapGridsFileName);
        string languageFilesPath = Path.Combine(path ?? _options.NostaleDataPath, _options.LanguageFileName);

        var datFile = _fileReader.ReadFileSystemFile<FileArchive>(datFilesPath);
        if (!datFile.IsSuccess)
        {
            return Result<NostaleFiles>.FromError(datFile);
        }

        var mapGridsFile = _fileReader.ReadFileSystemFile<FileArchive>(mapGridsFilesPath);
        if (!mapGridsFile.IsSuccess)
        {
            return Result<NostaleFiles>.FromError(mapGridsFile);
        }

        var languageFiles = new Dictionary<Language, FileArchive>();
        foreach (var language in languages.Concat(_options.SupportedLanguages))
        {
            var langString = language.ToString().ToLower();
            var langPath = languageFilesPath.Replace("%lang%", langString);
            var languageFile = _fileReader.ReadFileSystemFile<FileArchive>(langPath);

            if (!languageFile.IsSuccess)
            {
                return Result<NostaleFiles>.FromError(languageFile);
            }

            languageFiles.Add(language, languageFile.Entity.Content);
        }

        return new NostaleFiles(languageFiles, datFile.Entity.Content, mapGridsFile.Entity.Content);
    }

    /// <summary>
    /// Parse the nostale files.
    /// </summary>
    /// <param name="path">The path to the files.</param>
    /// <param name="languages">The languages to parse.</param>
    /// <returns>Parsed data or an error.</returns>
    public Result<NostaleData> ParseFiles(string? path = null, params Language[] languages)
    {
        try
        {
            if (_parsed is not null)
            {
                return _parsed;
            }

            var filesResult = GetFiles(path, languages);
            if (!filesResult.IsSuccess)
            {
                return Result<NostaleData>.FromError(filesResult);
            }
            var files = filesResult.Entity;

            var skillParser = new SkillParser();
            var skillsResult = skillParser.Parse(files);
            if (!skillsResult.IsSuccess)
            {
                return Result<NostaleData>.FromError(skillsResult);
            }

            var mapParser = new MapParser();
            var mapsResult = mapParser.Parse(files);
            if (!mapsResult.IsSuccess)
            {
                return Result<NostaleData>.FromError(mapsResult);
            }

            var itemParser = new ItemParser();
            var itemsResult = itemParser.Parse(files);
            if (!itemsResult.IsSuccess)
            {
                return Result<NostaleData>.FromError(itemsResult);
            }

            var monsterParser = new MonsterParser();
            var monstersResult = monsterParser.Parse(files);
            if (!monstersResult.IsSuccess)
            {
                return Result<NostaleData>.FromError(monstersResult);
            }

            var langParser = new LangParser();
            var translations
                = new Dictionary<Language, IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>();
            foreach (var language in files.LanguageFiles.Keys)
            {
                var languageParseResult = langParser.Parse(files, language);
                if (!languageParseResult.IsSuccess)
                {
                    return Result<NostaleData>.FromError(languageParseResult);
                }

                translations.Add(language, languageParseResult.Entity);
            }

            return _parsed = new NostaleData
            (
                translations,
                itemsResult.Entity,
                monstersResult.Entity,
                skillsResult.Entity,
                mapsResult.Entity
            );
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Create a language service from parsed files.
    /// </summary>
    /// <returns>A language service or an error.</returns>
    public Result<ILanguageService> CreateLanguageService()
    {
        var parsed = ParseFiles();
        if (!parsed.IsSuccess)
        {
            return Result<ILanguageService>.FromError(parsed);
        }

        return new LanguageService(parsed.Entity.Translations, _languageOptions);
    }

    /// <summary>
    /// Create an info service from parsed files.
    /// </summary>
    /// <returns>An info service or an error.</returns>
    public Result<IInfoService> CreateInfoService()
    {
        var parsed = ParseFiles();
        if (!parsed.IsSuccess)
        {
            return Result<IInfoService>.FromError(parsed);
        }

        return new InfoService(parsed.Entity);
    }
}