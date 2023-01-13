//
//  NostaleMapApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Apis.Unsafe;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Errors;
using Remora.Results;

namespace NosSmooth.Game.Apis.Safe;

/// <summary>
/// Packet api for managing maps in inventory.
/// </summary>
public class NostaleMapApi
{
    private readonly Game _game;
    private readonly UnsafeMapApi _unsafeMapApi;

    /// <summary>
    /// The range the player may pick up items in.
    /// </summary>
    public static short PickUpRange => 5;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleMapApi"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="unsafeMapApi">The unsafe map api.</param>
    public NostaleMapApi(Game game, UnsafeMapApi unsafeMapApi)
    {
        _game = game;
        _unsafeMapApi = unsafeMapApi;
    }

    /// <summary>
    /// Pick up the given item.
    /// </summary>
    /// <remarks>
    /// Checks that the item is in distance,
    /// if the character's position or
    /// item's position is not initialized, returns
    /// an error.
    /// </remarks>
    /// <param name="item">The item.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> CharacterPickUpAsync(GroundItem item, CancellationToken ct = default)
    {
        var character = _game.Character;
        if (character is null)
        {
            return new NotInitializedError("Game.Character");
        }

        var characterPosition = character.Position;
        if (characterPosition is null)
        {
            return new NotInitializedError("Game.Character.Position");
        }

        var itemPosition = item.Position;
        if (itemPosition is null)
        {
            return new NotInitializedError("item.Position");
        }

        if (!itemPosition.Value.IsInRange(characterPosition.Value, PickUpRange))
        {
            return new NotInRangeError("Character", characterPosition.Value, itemPosition.Value, PickUpRange);
        }

        return await _unsafeMapApi.CharacterPickUpAsync(item.Id, ct);
    }

    /// <summary>
    /// Pick up the given item.
    /// </summary>
    /// <remarks>
    /// Checks that the character has a pet company,
    /// that the item is in distance.
    /// When the pet's position or
    /// item's position is not initialized, returns
    /// an error.
    /// </remarks>
    /// <param name="item">The item.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> PetPickUpAsync(GroundItem item, CancellationToken ct = default)
    {
        var mates = _game.Mates;
        if (mates is null)
        {
            return new NotInitializedError("Game.Mates");
        }

        var pet = mates.CurrentPet;
        if (pet is null)
        {
            return new NotInitializedError("Game.Mates.CurrentPet");
        }

        var entity = _game.CurrentMap?.Entities.GetEntity(pet.Pet.MateId);
        if (entity is null)
        {
            return new NotInitializedError("Game.CurrentMap.Entities.PetEntity");
        }

        var petPosition = entity.Position;
        if (petPosition is null)
        {
            return new NotInitializedError("Game.CurrentMap.Entities.PetEntity.Position");
        }

        var itemPosition = item.Position;
        if (itemPosition is null)
        {
            return new NotInitializedError("item.Position");
        }

        if (!itemPosition.Value.IsInRange(petPosition.Value, PickUpRange))
        {
            return new NotInRangeError("Pet", petPosition.Value, itemPosition.Value, PickUpRange);
        }

        return await _unsafeMapApi.PetPickUpAsync(item.Id, ct);
    }

}