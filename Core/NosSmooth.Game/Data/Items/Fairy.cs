//
//  Fairy.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Items;

public record Fairy(int ItemVNum, Element Element, IItemInfo? Info) : Item(ItemVNum, Info);