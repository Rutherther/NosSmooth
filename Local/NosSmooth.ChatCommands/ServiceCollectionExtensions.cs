//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.LocalClient;
using NosSmooth.LocalClient.Extensions;
using Remora.Commands.Extensions;

namespace NosSmooth.ChatCommands;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NosTale commands and the interceptor to execute commands with.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="prefix">The prefix for the commands.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleChatCommands(this IServiceCollection serviceCollection, string prefix = "#")
    {
        serviceCollection
            .Configure<ChatCommandsOptions>((o) => o.Prefix = prefix);

        return serviceCollection
            .AddCommands()
            .Configure<LocalClientOptions>(o => o.AllowIntercept = true)
            .AddSingleton<FeedbackService>()
            .AddPacketInterceptor<ChatCommandInterceptor>();
    }
}