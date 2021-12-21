//
//  Position.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data;

/// <summary>
/// Represents nostale positition on map.
/// </summary>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
public record Position(int X, int Y);