//
//  EquipmentHelpers.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Items;
using NosSmooth.Packets.Server.Maps;
using NosSmooth.Packets.Server.Weapons;
using Remora.Results;

namespace NosSmooth.Game.Helpers;

/// <summary>
/// Helpers for creating equipment from packets.
/// </summary>
public static class EquipmentHelpers
{
    /// <summary>
    /// Create <see cref="Equipment"/> from the given in equipment subpacket.
    /// </summary>
    /// <param name="infoService">The info service.</param>
    /// <param name="equipmentSubPacket">The subpacket.</param>
    /// <param name="weaponUpgradeRare">The weapon upgrade.</param>
    /// <param name="armorUpgradeRare">The armor upgrade.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>The equipment or an error.</returns>
    public static async Task<Equipment> CreateEquipmentFromInSubpacketAsync
    (
        IInfoService infoService,
        InEquipmentSubPacket equipmentSubPacket,
        UpgradeRareSubPacket? weaponUpgradeRare = default,
        UpgradeRareSubPacket? armorUpgradeRare = default,
        CancellationToken ct = default
    )
    {
        var fairy = await CreateItemAsync(infoService, equipmentSubPacket.FairyVNum, ct);
        var mask = await CreateItemAsync(infoService, equipmentSubPacket.MaskVNum, ct);
        var hat = await CreateItemAsync(infoService, equipmentSubPacket.HatVNum, ct);
        var costumeSuit = await CreateItemAsync(infoService, equipmentSubPacket.CostumeSuitVNum, ct);
        var costumeHat = await CreateItemAsync(infoService, equipmentSubPacket.CostumeHatVNum, ct);
        var mainWeapon = await CreateItemAsync(infoService, equipmentSubPacket.MainWeaponVNum, weaponUpgradeRare, ct);
        var secondaryWeapon = await CreateItemAsync(infoService, equipmentSubPacket.SecondaryWeaponVNum, null, ct);
        var armor = await CreateItemAsync(infoService, equipmentSubPacket.ArmorVNum, armorUpgradeRare, ct);

        return new Equipment
        (
            hat,
            armor,
            mainWeapon,
            secondaryWeapon,
            mask,
            fairy,
            costumeSuit,
            costumeHat,
            equipmentSubPacket.WeaponSkin,
            equipmentSubPacket.WingSkin
        );
    }

    private static async Task<Item?> CreateItemAsync(IInfoService infoService, int? itemVNum, CancellationToken ct = default)
    {
        if (itemVNum is null)
        {
            return null;
        }

        var itemInfo = await infoService.GetItemInfoAsync(itemVNum.Value, ct);

        return new Item
        (
            itemVNum.Value,
            itemInfo.IsSuccess ? itemInfo.Entity : null
        );
    }

    private static async Task<UpgradeableItem?> CreateItemAsync
        (IInfoService infoService, int? itemVNum, UpgradeRareSubPacket? upgradeRareSubPacket, CancellationToken ct = default)
    {
        if (itemVNum is null)
        {
            return null;
        }

        var itemInfo = await infoService.GetItemInfoAsync(itemVNum.Value, ct);

        return new UpgradeableItem
        (
            itemVNum.Value,
            itemInfo.IsSuccess ? itemInfo.Entity : null,
            upgradeRareSubPacket?.Upgrade,
            upgradeRareSubPacket?.Rare
        );
    }
}