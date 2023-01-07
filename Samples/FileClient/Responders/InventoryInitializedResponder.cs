//
//  InventoryInitializedResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Game.Data.Items;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Inventory;
using NosSmooth.Game.Extensions;
using Remora.Results;

namespace FileClient.Responders;

/// <inheritdoc />
public class InventoryInitializedResponder : IGameResponder<InventoryInitializedEvent>
{
    private readonly ILanguageService _languageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryInitializedResponder"/> class.
    /// </summary>
    /// <param name="languageService">The langauge service.</param>
    public InventoryInitializedResponder(ILanguageService languageService)
    {
        _languageService = languageService;

    }

    /// <inheritdoc />
    public async Task<Result> Respond(InventoryInitializedEvent gameEvent, CancellationToken ct = default)
    {
        foreach (var bag in gameEvent.Inventory)
        {
            foreach (var slot in bag)
            {
                var item = slot.Item;
                if (item?.Info is not null && bag.Type != item.Info.BagType)
                {
                    var translatedResult = await _languageService.GetTranslationAsync(item.Info.Name, Language.Cz, ct);
                    var entity = translatedResult.Entity;

                    Console.WriteLine(entity + $", {item.ItemVNum} is: {bag.Type}, should be: {item.Info.BagType.Convert()}");
                }
            }
        }

        return Result.FromSuccess();
    }
}