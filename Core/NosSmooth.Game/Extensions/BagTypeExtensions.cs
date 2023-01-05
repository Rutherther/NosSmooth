//
//  BagTypeExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Data.Abstractions.Enums;

namespace NosSmooth.Game.Extensions;

/// <summary>
/// Extension methods for <see cref="BagType"/>, <see cref="Packets.Enums.Inventory.BagType"/>.
/// </summary>
public static class BagTypeExtensions
{
    /// <summary>
    /// Convert packets bag type to data bag type.
    /// </summary>
    /// <param name="bagType">The data bag type.</param>
    /// <returns>The packets bag type.</returns>
    public static BagType Convert(this Packets.Enums.Inventory.BagType bagType)
        => (BagType)(int)bagType;

    /// <summary>
    /// Convert data bag type to packets bag type.
    /// </summary>
    /// <param name="bagType">The packets bag type.</param>
    /// <returns>The data bag type.</returns>
    public static Packets.Enums.Inventory.BagType Convert(this BagType bagType)
        => (Packets.Enums.Inventory.BagType)(int)bagType;
}