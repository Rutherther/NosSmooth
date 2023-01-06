//
//  InventoryInitResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Data.Inventory;
using NosSmooth.Game.Data.Items;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Inventory;
using NosSmooth.Game.Extensions;
using NosSmooth.Packets.Server.Inventory;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Inventory;

/// <summary>
/// Initialize an inventory.
/// </summary>
public class InventoryInitResponder : IPacketResponder<InvPacket>, IPacketResponder<ExtsPacket>,
    IPacketResponder<IvnPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<InventoryInitResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryInitResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public InventoryInitResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<InventoryInitResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<InvPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        var slots = new List<InventorySlot>();

        foreach (var subPacket in packet.InvSubPackets)
        {
            slots.Add(await CreateSlot(subPacket, ct));
        }

        void AddItems(Data.Inventory.Inventory inv)
        {
            var converted = packet.Bag.Convert();
            var bag = inv.GetBag(converted);
            bag.Clear();

            foreach (var slot in slots)
            {
                bag.SetSlot(slot);
            }
        }

        var inventory = await _game.CreateOrUpdateInventoryAsync
        (
            () =>
            {
                var inv = new Data.Inventory.Inventory();
                AddItems(inv);
                return inv;
            },
            inv =>
            {
                AddItems(inv);
                return inv;
            },
            ct: ct
        );

        if (inventory is null)
        {
            throw new UnreachableException();
        }

        if (packet.Bag == Packets.Enums.Inventory.BagType.Costume)
        {
            // last bag initialized. TODO solve race condition.
            await _eventDispatcher.DispatchEvent
            (
                new InventoryInitializedEvent(inventory),
                ct
            );
        }

        return await _eventDispatcher.DispatchEvent
        (
            new InventoryBagInitializedEvent(inventory.GetBag(packet.Bag.Convert())),
            ct
        );
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<ExtsPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        void SetSlots(Data.Inventory.Inventory inv)
        {
            inv.GetBag(BagType.Main).Slots = packet.MainSlots;
            inv.GetBag(BagType.Equipment).Slots = packet.EquipmentSlots;
            inv.GetBag(BagType.Etc).Slots = packet.EtcSlots;
        }

        await _game.CreateOrUpdateInventoryAsync
        (
            () =>
            {
                var inv = new Data.Inventory.Inventory();
                SetSlots(inv);
                return inv;
            },
            inv =>
            {
                SetSlots(inv);
                return inv;
            },
            ct: ct
        );

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<IvnPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var slot = await CreateSlot(packet.InvSubPacket, ct);

        var inventory = await _game.CreateOrUpdateInventoryAsync
        (
            () =>
            {
                var inv = new Data.Inventory.Inventory();
                inv.GetBag(packet.Bag.Convert()).SetSlot(slot);
                return inv;
            },
            inv =>
            {
                inv.GetBag(packet.Bag.Convert()).SetSlot(slot);
                return inv;
            },
            ct: ct
        );

        if (inventory is null)
        {
            throw new UnreachableException();
        }

        return await _eventDispatcher.DispatchEvent
        (
            new InventorySlotUpdatedEvent
            (
                inventory.GetBag(packet.Bag.Convert()),
                slot
            ),
            ct
        );
    }

    private async Task<InventorySlot> CreateSlot(InvSubPacket packet, CancellationToken ct)
    {
        if (packet.VNum is null)
        {
            return new InventorySlot(packet.Slot, 0, null);
        }

        var itemInfoResult = await _infoService.GetItemInfoAsync(packet.VNum.Value, ct);

        if (!itemInfoResult.IsDefined(out var itemInfo))
        {
            _logger.LogWarning
                ("Could not obtain an item info for vnum {vnum}: {error}", packet.VNum, itemInfoResult.ToFullString());
        }

        // TODO: figure out other stuff from equipment inventory such as fairies
        if (itemInfo?.Type is ItemType.Weapon or ItemType.Armor)
        {
            var item = new UpgradeableItem
            (
                packet.VNum.Value,
                itemInfo,
                (byte?)packet.UpgradeOrDesign,
                (sbyte)packet.RareOrAmount,
                packet.RuneCount
            );
            return new InventorySlot(packet.Slot, 1, item);
        }
        else if (itemInfo?.Type is ItemType.Specialist)
        {
            var item = new SpItem
            (
                packet.VNum.Value,
                itemInfo,
                (sbyte?)packet.RareOrAmount,
                (byte?)packet.UpgradeOrDesign,
                packet.SpStoneUpgrade
            );
            return new InventorySlot(packet.Slot, 1, item);
        }
        else
        {
            var item = new Item(packet.VNum.Value, itemInfo);
            return new InventorySlot(packet.Slot, packet.RareOrAmount, item);
        }
    }
}