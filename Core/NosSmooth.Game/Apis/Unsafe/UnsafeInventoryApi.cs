//
//  UnsafeInventoryApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data;
using NosSmooth.Core.Client;
using NosSmooth.Core.Contracts;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Inventory;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Packets.Client.Inventory;
using NosSmooth.Packets.Enums.Inventory;
using NosSmooth.Packets.Server.Maps;
using OneOf.Types;
using Remora.Results;

namespace NosSmooth.Game.Apis.Unsafe;

/// <summary>
/// Packet api for managing items in inventory.
/// </summary>
public class UnsafeInventoryApi
{
    private readonly ManagedNostaleClient _client;
    private readonly Contractor _contractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeInventoryApi"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    /// <param name="contractor">The contractor.</param>
    public UnsafeInventoryApi(ManagedNostaleClient client, Contractor contractor)
    {
        _client = client;
        _contractor = contractor;
    }

    /// <summary>
    /// Drop the given item.
    /// </summary>
    /// <param name="bag">The bag where the item is located.</param>
    /// <param name="slot">The slot the item is at.</param>
    /// <param name="amount">The amount to drop.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> DropItemAsync
    (
        BagType bag,
        short slot,
        short amount,
        CancellationToken ct = default
    )
        => _client.SendPacketAsync(new PutPacket(bag, slot, amount), ct);

    /// <summary>
    /// Creates a contract for dropping an item.
    /// Returns the ground item that was thrown on the ground.
    /// </summary>
    /// <param name="bag">The inventory bag.</param>
    /// <param name="slot">The inventory slot.</param>
    /// <param name="amount">The amount to drop.</param>
    /// <returns>A contract representing the drop operation.</returns>
    public IContract<GroundItem, DefaultStates> ContractDropItem
    (
        BagType bag,
        InventorySlot slot,
        short amount
    )
    {
        // TODO: confirm dialog.
        return new ContractBuilder<GroundItem, DefaultStates, NoErrors>(_contractor, DefaultStates.None)
            .SetMoveAction
            (
                DefaultStates.None,
                async (_, ct) =>
                {
                    await DropItemAsync(bag, slot.Slot, amount, ct);
                    return true;
                },
                DefaultStates.Requested
            )
            .SetMoveFilter<ItemDroppedEvent>
            (
                DefaultStates.Requested,
                data => data.Item.Amount == amount && data.Item.VNum == slot.Item?.ItemVNum,
                DefaultStates.ResponseObtained
            )
            .SetFillData<ItemDroppedEvent>
            (
                DefaultStates.Requested,
                data => data.Item
            )
            .Build();
    }

    /// <summary>
    /// Move the given item within one bag.
    /// </summary>
    /// <param name="bag">The bag the item is in.</param>
    /// <param name="sourceSlot">The source slot to move the item from.</param>
    /// <param name="destinationSlot">The destination slot to move the item to.</param>
    /// <param name="amount">The amount to move.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> MoveItemAsync
    (
        BagType bag,
        short sourceSlot,
        short destinationSlot,
        short amount,
        CancellationToken ct = default
    )
        => MoveItemAsync
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
    public Task<Result> MoveItemAsync
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
            return _client.SendPacketAsync(new MviPacket(sourceBag, sourceSlot, amount, destinationSlot), ct);
        }

        return _client.SendPacketAsync(new MvePacket(sourceBag, sourceSlot, destinationBag, destinationSlot), ct);
    }
}