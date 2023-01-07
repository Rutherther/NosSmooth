//
//  NostaleMapPacketApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Game.Attributes;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Items;
using NosSmooth.Game.Errors;
using NosSmooth.Packets.Client.Inventory;
using NosSmooth.Packets.Client.Movement;
using NosSmooth.Packets.Enums.Entities;
using Remora.Results;

namespace NosSmooth.Game.Apis;

/// <summary>
/// Packet api for managing maps in inventory.
/// </summary>
public class NostaleMapPacketApi
{
    /// <summary>
    /// The range the player may pick up items in.
    /// </summary>
    public static short PickUpRange => 5;

    private readonly Game _game;
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleMapPacketApi"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="client">The client.</param>
    public NostaleMapPacketApi(Game game, INostaleClient client)
    {
        _game = game;
        _client = client;
    }

    /// <summary>
    /// Use the given portal.
    /// </summary>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    [Unsafe("Portal position not checked.")]
    public async Task<Result> UsePortalAsync(CancellationToken ct = default)
        => await _client.SendPacketAsync(new PreqPacket(), ct);

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

        return await CharacterPickUpAsync(item.Id, ct);
    }

    /// <summary>
    /// Pick up the given item by character.
    /// </summary>
    /// <remarks>
    /// Unsafe, does not check anything.
    /// </remarks>
    /// <param name="itemId">The id of the item.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    [Unsafe("Nor character distance, nor the existence of item is checked.")]
    public async Task<Result> CharacterPickUpAsync(long itemId, CancellationToken ct = default)
    {
        var character = _game.Character;
        if (character is null)
        {
            return new NotInitializedError("Character");
        }

        return await _client.SendPacketAsync(new GetPacket(EntityType.Player, character.Id, itemId), ct);
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

        return await PetPickUpAsync(item.Id, ct);
    }

    /// <summary>
    /// Pick up the given item by pet.
    /// </summary>
    /// <remarks>
    /// Unsafe, does not check anything.
    /// </remarks>
    /// <param name="itemId">The id of the item.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    [Unsafe("Nor pet distance to item nor whether the item exists is checked.")]
    public async Task<Result> PetPickUpAsync(long itemId, CancellationToken ct = default)
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

        return await _client.SendPacketAsync(new GetPacket(EntityType.Player, pet.Pet.MateId, itemId), ct);
    }
}