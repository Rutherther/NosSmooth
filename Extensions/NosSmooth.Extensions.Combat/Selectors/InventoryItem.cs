//
//  InventoryItem.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Data.Inventory;

namespace NosSmooth.Extensions.Combat.Selectors;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Fix")]
public record struct InventoryItem(BagType Bag, InventorySlot Item);