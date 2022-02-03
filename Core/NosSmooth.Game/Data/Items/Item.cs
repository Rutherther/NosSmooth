//
//  Item.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Game.Data.Items;

/// <summary>
/// A NosTale item.
/// </summary>
/// <param name="ItemVNum"></param>
/// <param name="Info"></param>
public record Item(int ItemVNum, IItemInfo? Info);