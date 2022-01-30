//
//  NostaleFiles.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Files;

namespace NosSmooth.Data.NOSFiles;

/// <summary>
/// Contains some of the NosTale NOS file archives.
/// </summary>
public class NostaleFiles
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleFiles"/> class.
    /// </summary>
    /// <param name="languageFiles">The language files.</param>
    /// <param name="datFiles">The dat files.</param>
    /// <param name="mapGridsFiles">The map grids files.</param>
    public NostaleFiles(IReadOnlyDictionary<Language, FileArchive> languageFiles, FileArchive datFiles, FileArchive mapGridsFiles)
    {
        LanguageFiles = languageFiles;
        DatFiles = datFiles;
        MapGridsFiles = mapGridsFiles;
    }

    /// <summary>
    /// Gets the file archives containing language text files.
    /// </summary>
    public IReadOnlyDictionary<Language, FileArchive> LanguageFiles { get; }

    /// <summary>
    /// Gets the dat files archive.
    /// </summary>
    public FileArchive DatFiles { get; }

    /// <summary>
    /// Gets the map grids files archive.
    /// </summary>
    public FileArchive MapGridsFiles { get; }
}