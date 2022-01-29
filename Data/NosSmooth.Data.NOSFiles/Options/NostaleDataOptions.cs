//
//  NostaleDataOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.NOSFiles.Options;

/// <summary>
/// Options for loading nostale files.
/// </summary>
public class NostaleDataOptions
{
    /// <summary>
    /// Gets or sets the supported languages that will be loaded into the memory..
    /// </summary>
    public Language[] SupportedLanguages { get; set; } = new Language[] { Language.En };

    /// <summary>
    /// Gets or sets the path to .nos files.
    /// </summary>
    public string NostaleDataPath { get; set; } = "NostaleData";

    /// <summary>
    /// Gets or sets the name of the file with map grid data.
    /// </summary>
    public string MapGridsFileName { get; set; } = "NStcData.NOS";

    /// <summary>
    /// Gets or sets the name of the file with infos data.
    /// </summary>
    public string InfosFileName { get; set; } = "NSgtdData.NOS";

    /// <summary>
    /// Gets or sets the name of the file with language data.
    /// </summary>
    /// <remarks>
    /// Should contain %lang% string to be replaced with the language abbrev.
    /// </remarks>
    public string LanguageFileName { get; set; } = "NSlangData_%lang%.NOS";
}