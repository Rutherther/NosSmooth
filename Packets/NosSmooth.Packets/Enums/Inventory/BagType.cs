//
//  BagType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Inventory;

/// <summary>
/// A bag in an inventory storing a given type of items.
/// </summary>
public enum BagType
{
    /// <summary>
    /// Equipment bag (first bag inside regular inventory).
    /// </summary>
    Equipment = 0,

    /// <summary>
    /// Main bag (second bag inside regular inventory).
    /// </summary>
    Main = 1,

    /// <summary>
    /// Etc bag (first bag inside the inventory).
    /// </summary>
    Etc = 2,

    /// <summary>
    /// Miniland bag (can be seen after pressing L).
    /// </summary>
    Miniland = 3,

    /// <summary>
    /// Specialist bag (last bag inside regular inventory, in a new window).
    /// </summary>
    Specialist = 6,

    /// <summary>
    /// Costume bag (last bag inside regular inventory, in a new window, right under specialist tab.).
    /// </summary>
    Costume = 7
}