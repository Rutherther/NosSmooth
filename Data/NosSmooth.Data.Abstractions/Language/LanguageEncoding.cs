//
//  LanguageEncoding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace NosSmooth.Data.Abstractions.Language;

/// <summary>
/// Mapping of language encoding.
/// </summary>
public class LanguageEncoding
{
    /// <summary>
    /// Get encoding for the given language.
    /// </summary>
    /// <param name="lang">The language.</param>
    /// <returns>An encoding.</returns>
    public static Encoding GetEncoding(Language lang)
    {
        switch (lang)
        {
            case Language.Tr:
                return CodePagesEncodingProvider.Instance.GetEncoding(1254) ?? Encoding.ASCII;
            case Language.Uk:
            case Language.Es:
                return CodePagesEncodingProvider.Instance.GetEncoding(1252) ?? Encoding.ASCII;
            case Language.Cz:
            case Language.De:
            case Language.Pl:
            case Language.It:
                return CodePagesEncodingProvider.Instance.GetEncoding(1250) ?? Encoding.ASCII;
        }

        return Encoding.ASCII;
    }
}