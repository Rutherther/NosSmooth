//
//  ItemInfoCommand.cs
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
/// Command for getting information about an item.
/// </summary>
public class ItemInfoCommand : CommandGroup
{
    private readonly IInfoService _infoService;
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemInfoCommand"/> class.
    /// </summary>
    /// <param name="infoService">The info service.</param>
    /// <param name="languageService">The language service.</param>
    public ItemInfoCommand(IInfoService infoService, ILanguageService languageService)
    {
        _infoService = infoService;
        _languageService = languageService;
    }

    /// <summary>
    /// Gets the info about the specified item.
    /// </summary>
    /// <param name="vnum">The vnum of the item.</param>
    /// <param name="language">The language.</param>
    /// <returns>A result that may or may not succeeded.</returns>
    [Command("item")]
    public async Task<Result> HandleItemInfo(int vnum, Language language = Language.Uk)
    {
        var itemResult = await _infoService.GetItemInfoAsync(vnum, CancellationToken);
        if (!itemResult.IsSuccess)
        {
            return Result.FromError(itemResult);
        }

        var translationResult = await _languageService.GetTranslationAsync
            (itemResult.Entity.Name, language, CancellationToken);
        if (!translationResult.IsSuccess)
        {
            return Result.FromError(translationResult);
        }

        Console.WriteLine("Found item {0}", translationResult.Entity);
        Console.WriteLine("  Type: {0}", itemResult.Entity.Type);
        Console.WriteLine("  SubType: {0}", itemResult.Entity.SubType);
        Console.WriteLine("  BagType: {0}", itemResult.Entity.BagType);
        Console.WriteLine("  Data: {0}", string.Join('|', itemResult.Entity.Data));

        return Result.FromSuccess();
    }
}