//
//  MonsterInfoCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Language;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace DataBrowser.Commands;

/// <summary>
/// Command for getting information about a monster.
/// </summary>
public class MonsterInfoCommand : CommandGroup
{
    private readonly IInfoService _infoService;
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonsterInfoCommand"/> class.
    /// </summary>
    /// <param name="infoService">The info service.</param>
    /// <param name="languageService">The language service.</param>
    public MonsterInfoCommand(IInfoService infoService, ILanguageService languageService)
    {
        _infoService = infoService;
        _languageService = languageService;
    }

    /// <summary>
    /// Gets the info about the specified monster.
    /// </summary>
    /// <param name="vnum">The vnum of the Monster.</param>
    /// <param name="language">The language.</param>
    /// <returns>A result that may or may not succeeded.</returns>
    [Command("monster")]
    public async Task<Result> HandleMonsterInfo(int vnum, Language language = Language.Uk)
    {
        var monsterResult = await _infoService.GetMonsterInfoAsync(vnum, CancellationToken);
        if (!monsterResult.IsSuccess)
        {
            return Result.FromError(monsterResult);
        }

        var translationResult = await _languageService.GetTranslationAsync
            (monsterResult.Entity.Name, language, CancellationToken);
        if (!translationResult.IsSuccess)
        {
            return Result.FromError(translationResult);
        }

        Console.WriteLine("Found Monster {0}", translationResult.Entity);
        Console.WriteLine("  Level: {0}", monsterResult.Entity.Level);

        return Result.FromSuccess();
    }
}