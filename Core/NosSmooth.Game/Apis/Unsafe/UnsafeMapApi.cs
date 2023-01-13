//
//  UnsafeMapApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Game.Errors;
using NosSmooth.Packets.Client.Inventory;
using NosSmooth.Packets.Client.Movement;
using NosSmooth.Packets.Enums.Entities;
using Remora.Results;

namespace NosSmooth.Game.Apis.Unsafe;

/// <summary>
/// Packet api for managing maps in inventory.
/// </summary>
public class UnsafeMapApi
{
    private readonly Game _game;
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeMapApi"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="client">The client.</param>
    public UnsafeMapApi(Game game, INostaleClient client)
    {
        _game = game;
        _client = client;
    }

    /// <summary>
    /// Use the given portal.
    /// </summary>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UsePortalAsync(CancellationToken ct = default)
        => _client.SendPacketAsync(new PreqPacket(), ct);

    /// <summary>
    /// Pick up the given item by character.
    /// </summary>
    /// <remarks>
    /// Unsafe, does not check anything.
    /// </remarks>
    /// <param name="itemId">The id of the item.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
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
    /// Pick up the given item by pet.
    /// </summary>
    /// <remarks>
    /// Unsafe, does not check anything.
    /// </remarks>
    /// <param name="itemId">The id of the item.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
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