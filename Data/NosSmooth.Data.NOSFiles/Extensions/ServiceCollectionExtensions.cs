//
//  ServiceCollectionExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Readers;
using NosSmooth.Data.NOSFiles.Readers.Types;

namespace NosSmooth.Data.NOSFiles.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the nostale file data info and language service.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleDataFiles(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddNostaleDataParsing()
            .AddSingleton<NostaleDataFilesManager>()
            .AddSingleton<IInfoService>(p => p.GetRequiredService<NostaleDataFilesManager>().InfoService)
            .AddSingleton<ILanguageService>(p => p.GetRequiredService<NostaleDataFilesManager>().LanguageService);
    }

    /// <summary>
    /// Adds the <see cref="NostaleDataParser"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddNostaleDataParsing(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddFileReader()
            .AddSingleton<NostaleDataParser>();
    }

    /// <summary>
    /// Add the file reader and NosTale type readers.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddFileReader(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<FileReader>()
            .AddFileTypeReader<NosZlibFileTypeReader>()
            .AddFileTypeReader<NosTextFileTypeReader>();
    }

    /// <summary>
    /// Add the given file type reader.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TTypeReader">The type of the reader.</typeparam>
    /// <returns>The collection.</returns>
    public static IServiceCollection AddFileTypeReader<TTypeReader>(this IServiceCollection serviceCollection)
        where TTypeReader : class, IFileTypeReader
    {
        return serviceCollection.AddSingleton<IFileTypeReader, TTypeReader>();
    }
}