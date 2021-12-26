//
//  Miniland.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Maps;

/// <summary>
/// Represents Miniland map that can contain miniland objects.
/// </summary>
/// <param name="Objects">The objects in the miniland.</param>
public record Miniland(IReadOnlyList<MinilandObject>? Objects) : Map;