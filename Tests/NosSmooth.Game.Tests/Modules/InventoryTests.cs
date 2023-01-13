//
//  InventoryTests.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Data.Abstractions.Enums;
using Xunit.Abstractions;

namespace NosSmooth.Game.Tests.Modules;

/// <summary>
/// Tests for Game.Inventory.
/// </summary>
public class InventoryTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public InventoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests empty inventory initialization.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_Initialize_Empty()
    {
        var data = PacketFileClient.CreateFor<InventoryTests>("init_empty", _testOutputHelper);

        await data.Client.ExecuteToEnd();

        data.Game.Inventory.ShouldNotBeNull();
        foreach (var bag in data.Game.Inventory)
        {
            foreach (var slot in bag)
            {
                throw new Exception("There is something in the inventory, but it should have been empty.");
            }
        }
    }

    /// <summary>
    /// Tests inventory initialization with a few things.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_Initialize_FewThings()
    {
        var data = PacketFileClient.CreateFor<InventoryTests>("init_move", _testOutputHelper);

        await data.Client.ExecuteUntilLabelAsync("AFTER_INVENTORY_INITIALIZED");

        data.Game.Inventory.ShouldNotBeNull();

        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(0).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(1).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(2).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(0).Item!.ItemVNum.ShouldBe(1);
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(1).Item!.ItemVNum.ShouldBe(2);
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(2).Item!.ItemVNum.ShouldBe(4);

        data.Game.Inventory.GetBag(BagType.Main).GetSlot(0).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Main).GetSlot(0).Item!.ItemVNum.ShouldBe(1012);
        data.Game.Inventory.GetBag(BagType.Main).GetSlot(0).Amount.ShouldBe((short)10);

        data.Game.Inventory.GetBag(BagType.Etc).GetSlot(0).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Etc).GetSlot(0).Item!.ItemVNum.ShouldBe(2018);
        data.Game.Inventory.GetBag(BagType.Etc).GetSlot(0).Amount.ShouldBe((short)10);
    }

    /// <summary>
    /// Tests inventory initialization with a few things.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Test_Move()
    {
        var data = PacketFileClient.CreateFor<InventoryTests>("init_move", _testOutputHelper);

        await data.Client.ExecuteUntilLabelAsync("AFTER_INVENTORY_MOVES");

        data.Game.Inventory.ShouldNotBeNull();

        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(6).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(7).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(8).Item.ShouldNotBeNull();
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(6).Item!.ItemVNum.ShouldBe(1);
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(7).Item!.ItemVNum.ShouldBe(2);
        data.Game.Inventory.GetBag(BagType.Equipment).GetSlot(8).Item!.ItemVNum.ShouldBe(4);
    }
}