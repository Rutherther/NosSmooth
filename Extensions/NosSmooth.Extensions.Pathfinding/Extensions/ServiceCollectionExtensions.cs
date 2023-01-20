//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Extensions;
using NosSmooth.Extensions.Pathfinding.Responders;
using NosSmooth.Packets.Server.Character;
using NosSmooth.Packets.Server.Maps;

namespace NosSmooth.Extensions.Pathfinding.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NosTale pathfinding using <see cref="Pathfinder"/> and <see cref="WalkManager"/>.
    /// </summary>
    /// <remarks>
    /// Finds and walks a given path.
    /// </remarks>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostalePathfinding(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddPacketResponder<AtResponder>()
            .AddPacketResponder<CMapResponder>()
            .AddPacketResponder<WalkResponder>()
            .AddPacketResponder<SuPacketResponder>()
            .AddPacketResponder<TpPacketResponder>()
            .AddPacketResponder<CInfoPacketResponder>()
            .AddSingleton<WalkManager>()
            .AddSingleton<Pathfinder>()
            .AddSingleton<PathfinderState>();
    }
}