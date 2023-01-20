//
//  PathfinderState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Stateful;
using NosSmooth.Data.Abstractions.Infos;

namespace NosSmooth.Extensions.Pathfinding;

/// <summary>
/// State of the <see cref="Pathfinder"/>.
/// </summary>
public class PathfinderState : IStatefulEntity
{
    /// <summary>
    /// Gets or sets the current map id.
    /// </summary>
    internal int? MapId { get; set; }

    /// <summary>
    /// Gets or sets the current map information.
    /// </summary>
    internal IMapInfo? MapInfo { get; set; }

    /// <summary>
    /// Gets or sets the current x.
    /// </summary>
    internal short X { get; set; }

    /// <summary>
    /// Gets or sets the current y.
    /// </summary>
    internal short Y { get; set; }

    /// <summary>
    /// Gets or sets the id of the charcter.
    /// </summary>
    internal long CharacterId { get; set; }
}