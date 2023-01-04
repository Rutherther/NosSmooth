//
//  Fairy.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Items;

/// <summary>
/// Information about a fairy.
/// </summary>
/// <param name="ItemVNum">The fairy vnum.</param>
/// <param name="Element">The element of the fairy.</param>
/// <param name="Info">The item information from data assembly.</param>
public record Fairy(int ItemVNum, Element Element, IItemInfo? Info) : Item(ItemVNum, Info);