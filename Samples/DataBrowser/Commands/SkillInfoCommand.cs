//
//  SkillInfoCommand.cs
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
/// Command for getting information about a Skill.
/// </summary>
public class SkillInfoCommand : CommandGroup
{
    private readonly IInfoService _infoService;
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillInfoCommand"/> class.
    /// </summary>
    /// <param name="infoService">The info service.</param>
    /// <param name="languageService">The language service.</param>
    public SkillInfoCommand(IInfoService infoService, ILanguageService languageService)
    {
        _infoService = infoService;
        _languageService = languageService;
    }

    /// <summary>
    /// Gets the info about the specified skill.
    /// </summary>
    /// <param name="vnum">The vnum of the Skill.</param>
    /// <param name="language">The language.</param>
    /// <returns>A result that may or may not succeeded.</returns>
    [Command("skill")]
    public async Task<Result> HandleSkillInfo(int vnum, Language language = Language.Uk)
    {
        var skillResult = await _infoService.GetSkillInfoAsync(vnum, CancellationToken);
        if (!skillResult.IsSuccess)
        {
            return Result.FromError(skillResult);
        }

        var translationResult = await _languageService.GetTranslationAsync
            (skillResult.Entity.Name, language, CancellationToken);
        if (!translationResult.IsSuccess)
        {
            return Result.FromError(translationResult);
        }

        Console.WriteLine("Found Skill {0}", translationResult.Entity);
        Console.WriteLine("  CastId: {0}", skillResult.Entity.CastId);
        Console.WriteLine("  MpCost: {0}", skillResult.Entity.MpCost);
        Console.WriteLine("  Cooldown: {0}", skillResult.Entity.Cooldown);
        Console.WriteLine("  Range: {0}", skillResult.Entity.Range);
        Console.WriteLine("  ZoneRange: {0}", skillResult.Entity.ZoneRange);
        Console.WriteLine("  HitType: {0}", skillResult.Entity.HitType);
        Console.WriteLine("  SkillType: {0}", skillResult.Entity.SkillType);
        Console.WriteLine("  TargetType: {0}", skillResult.Entity.TargetType);

        return Result.FromSuccess();
    }
}