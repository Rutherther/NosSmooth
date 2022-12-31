//
//  UseItemPolicy.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Selectors;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Items;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Policies;

/// <summary>
/// The policy to use an item.
/// </summary>
/// <param name="UseItems">Whether to use items.</param>
/// <param name="UseBelowHealthPercentage">Use items below the given character's health percentage.</param>
/// <param name="UseBelowManaPercentage">Use items below the given character's mana percentage.</param>
/// <param name="UseHealthItemsVNums">The vnums of the items to use as health items.</param>
/// <param name="UseManaItemsVNums">The vnums of the items to use as mana items.</param>
public record UseItemPolicy
(
    bool UseItems,
    int UseBelowHealthPercentage,
    int UseBelowManaPercentage,
    int[] UseHealthItemsVNums,
    int[] UseManaItemsVNums
) : IItemSelector
{
    /// <inheritdoc />
    public Result<Item> GetSelectedItem(ICombatState combatState, ICollection<Item> possibleItems)
    {
        var character = combatState.Game.Character;
        if (character is null)
        {
            return new ItemNotFoundError();
        }

        if (ShouldUseHpItem(character))
        {
            var item = possibleItems.FirstOrDefault(x => UseHealthItemsVNums.Contains(x.ItemVNum));
            if (item is not null)
            {
                return item;
            }
        }

        if (ShouldUseMpItem(character))
        {
            var item = possibleItems.FirstOrDefault(x => UseManaItemsVNums.Contains(x.ItemVNum));
            if (item is not null)
            {
                return item;
            }
        }

        return new ItemNotFoundError();
    }

    /// <inheritdoc />
    public Result<bool> ShouldUseItem(ICombatState combatState)
    {
        if (!UseItems)
        {
            return false;
        }

        var character = combatState.Game.Character;
        if (character is null)
        {
            return false;
        }

        return ShouldUseHpItem(character) || ShouldUseMpItem(character);
    }

    private bool ShouldUseHpItem(Character character)
    {
        return character.Hp is not null && character.Hp.Percentage is not null
            && character.Hp.Percentage < UseBelowHealthPercentage;
    }

    private bool ShouldUseMpItem(Character character)
    {
        return character.Mp is not null && character.Mp.Percentage is not null
            && character.Mp.Percentage < UseBelowManaPercentage;
    }
}