//
//  LangParser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Parsers;

/// <summary>
/// Language txt file parser.
/// </summary>
public class LangParser
{
    /// <summary>
    /// Parse the given language.
    /// </summary>
    /// <param name="files">The NosTale files.</param>
    /// <param name="language">The language to parse.</param>
    /// <returns>Translations or an error.</returns>
    public Result<IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>> Parse(NostaleFiles files, Language language)
    {
        if (!files.LanguageFiles.ContainsKey(language))
        {
            return new NotFoundError($"Could not find the language file for {language}.");
        }

        var archive = files.LanguageFiles[language];
        var encoding = LanguageEncoding.GetEncoding(language);
        var dictionary = new Dictionary<TranslationRoot, IReadOnlyDictionary<string, string>>();

        var itemParsedResult = ParseFile(archive, encoding, $"_code_{language.ToString().ToLower()}_Item.txt");
        if (!itemParsedResult.IsSuccess)
        {
            return Result<IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>.FromError
                (itemParsedResult);
        }
        dictionary.Add(TranslationRoot.Item, itemParsedResult.Entity);

        var monsterParsedResult = ParseFile(archive, encoding, $"_code_{language.ToString().ToLower()}_monster.txt");
        if (!monsterParsedResult.IsSuccess)
        {
            return Result<IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>.FromError
                (monsterParsedResult);
        }
        dictionary.Add(TranslationRoot.Monster, monsterParsedResult.Entity);

        var skillParsedResult = ParseFile(archive, encoding, $"_code_{language.ToString().ToLower()}_Skill.txt");
        if (!skillParsedResult.IsSuccess)
        {
            return Result<IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>.FromError
                (skillParsedResult);
        }
        dictionary.Add(TranslationRoot.Skill, skillParsedResult.Entity);

        var mapParsedResult = ParseFile(archive, encoding, $"_code_{language.ToString().ToLower()}_MapIDData.txt");
        if (!mapParsedResult.IsSuccess)
        {
            return Result<IReadOnlyDictionary<TranslationRoot, IReadOnlyDictionary<string, string>>>.FromError
                (mapParsedResult);
        }
        dictionary.Add(TranslationRoot.Map, mapParsedResult.Entity);

        return dictionary;
    }

    private Result<IReadOnlyDictionary<string, string>> ParseFile(FileArchive files, Encoding encoding, string name)
    {
        var fileResult = files.FindFile(name);
        if (!fileResult.IsSuccess)
        {
            return Result<IReadOnlyDictionary<string, string>>.FromError(fileResult);
        }
        var fileContent = encoding.GetString(fileResult.Entity.Content);

        var dictionary = new Dictionary<string, string>();
        var lines = fileContent.Split('\r', '\n');
        foreach (var line in lines)
        {
            var splitted = line.Split('\t');
            if (splitted.Length != 2)
            {
                continue;
            }

            dictionary.Add(splitted[0], splitted[1]);
        }

        return dictionary;
    }
}