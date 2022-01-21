//
//  LanguageKey.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Language;

/// <summary>
/// Key for language translation.
/// </summary>
public struct LanguageKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageKey"/> struct.
    /// </summary>
    /// <param name="key">The key num.</param>
    public LanguageKey(long key)
        : this($"zts{key}e")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageKey"/> struct.
    /// </summary>
    /// <param name="key">The key.</param>
    public LanguageKey(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key { get; }
}