//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Database.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NosTale data language and info services using a database.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleDataDatabase
        (this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IInfoService, DatabaseInfoService>()
            .AddSingleton<ILanguageService, DatabaseLanguageService>()
            .AddDbContextFactory<NostaleDataContext>
            (
                (provider, builder) => builder.UseSqlite
                    (provider.GetRequiredService<IOptions<NostaleDataContextOptions>>().Value.ConnectionString)
            );
    }

    /// <summary>
    /// Adds <see cref="DatabaseMigrator"/> used for migrating data to the database.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleDatabaseMigrator(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddNostaleDataDatabase()
            .AddSingleton<DatabaseMigrator>();
    }
}