//
//  IItemInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Abstractions.Infos;

/// <summary>
/// The NosTale item information.
/// </summary>
public interface IItemInfo : IVNumInfo
{
    /// <summary>
    /// Gets the translatable name of the item.
    /// </summary>
    TranslatableString Name { get; }

    /// <summary>
    /// Gets the type of the item. TODO UNKNOWN.
    /// </summary>
    int Type { get; }

    /// <summary>
    /// Gets the subtype of the item. TODO UNKNOWN.
    /// </summary>
    int SubType { get; }

    /// <summary>
    /// Gets the bag the item belongs to.
    /// </summary>
    BagType BagType { get; }

    /// <summary>
    /// Gets the data of the item.
    /// </summary>
    string[] Data { get; }
}