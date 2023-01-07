//
//  NostaleInventoryPacketApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Packets.Client.Inventory;
using NosSmooth.Packets.Enums.Inventory;
using Remora.Results;

namespace NosSmooth.Game.Apis;

/// <summary>
/// Packet api for managing items in inventory.
/// </summary>
public class NostaleInventoryPacketApi
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleInventoryPacketApi"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    public NostaleInventoryPacketApi(INostaleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Drop the given item.
    /// </summary>
    /// <param name="bag">The bag where the item is located.</param>
    /// <param name="slot">The slot the item is at.</param>
    /// <param name="amount">The amount to drop.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> DropItemAsync
    (
        BagType bag,
        short slot,
        short amount,
        CancellationToken ct = default
    )
        => await _client.SendPacketAsync(new PutPacket(bag, slot, amount), ct);

    /// <summary>
    /// Move the given item within one bag.
    /// </summary>
    /// <param name="bag">The bag the item is in.</param>
    /// <param name="sourceSlot">The source slot to move the item from.</param>
    /// <param name="destinationSlot">The destination slot to move the item to.</param>
    /// <param name="amount">The amount to move.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> MoveItemAsync
    (
        BagType bag,
        short sourceSlot,
        short destinationSlot,
        short amount,
        CancellationToken ct = default
    )
        => await MoveItemAsync
        (
            bag,
            sourceSlot,
            bag,
            destinationSlot,
            amount,
            ct
        );

    /// <summary>
    /// Move an item from the given source bag and slot to the given destination bag and slot.
    /// </summary>
    /// <param name="sourceBag">The bag the item is in.</param>
    /// <param name="sourceSlot">The source slot to move the item from.</param>
    /// <param name="destinationBag">The destination bag to move the item to.</param>
    /// <param name="destinationSlot">The destination slot to move the item to.</param>
    /// <param name="amount">The amount to move.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> MoveItemAsync
    (
        BagType sourceBag,
        short sourceSlot,
        BagType destinationBag,
        short destinationSlot,
        short amount,
        CancellationToken ct = default
    )
    {
        if (sourceBag == destinationBag)
        {
            return await _client.SendPacketAsync(new MviPacket(sourceBag, sourceSlot, amount, destinationSlot), ct);
        }

        return await _client.SendPacketAsync(new MvePacket(sourceBag, sourceSlot, destinationBag, destinationSlot), ct);
    }
}