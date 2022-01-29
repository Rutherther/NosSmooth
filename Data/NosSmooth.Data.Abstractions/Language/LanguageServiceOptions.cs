//
//  LanguageServiceOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Language;

/// <summary>
/// Options for <see cref="ILanguageService"/>.
/// </summary>
public class LanguageServiceOptions
{
    /// <summary>
    /// Get or sets the default language.
    /// </summary>
    public Language Language { get; set; } = Language.Uk;

    /// <summary>
    /// Gets or sets whether to fill <see cref="TranslatableString"/> translations with the default language.
    /// </summary>
    public bool FillTranslatableStrings { get; set; } = true;
}