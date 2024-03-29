﻿//
//  InventorySlot.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Items;

namespace NosSmooth.Game.Data.Inventory;

/// <summary>
/// Represents item in bag inventory of the character.
/// </summary>
public record InventorySlot(short Slot, short Amount, Item? Item);