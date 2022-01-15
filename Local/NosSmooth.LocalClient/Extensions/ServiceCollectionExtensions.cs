//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalBinding.Extensions;
using NosSmooth.LocalClient.CommandHandlers.Walk;

namespace NosSmooth.LocalClient.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="NostaleLocalClient"/> along with all core dependencies.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddLocalClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddNostaleCore();
        serviceCollection.AddNostaleBindings();
        serviceCollection
            .AddCommandHandler<WalkCommandHandler>()
            .AddPacketResponder<WalkPacketResponder>()
            .AddSingleton<WalkStatus>();
        serviceCollection.TryAddSingleton<NostaleLocalClient>();
        serviceCollection.TryAddSingleton<INostaleClient>(p => p.GetRequiredService<NostaleLocalClient>());

        return serviceCollection;
    }
}