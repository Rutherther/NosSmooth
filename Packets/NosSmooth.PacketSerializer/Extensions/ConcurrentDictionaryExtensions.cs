//
//  ConcurrentDictionaryExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using Remora.Results;

namespace NosSmooth.Packets.Extensions;

/// <summary>
/// Extension methods for <see cref="ConcurrentDictionary{TKey,TValue}"/>.
/// </summary>
public static class ConcurrentDictionaryExtensions
{
    /// <summary>
    /// Adds the value from the factory if it doesn't exist already,
    /// otherwise returns the existing avlue.
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key to add the value at.</param>
    /// <param name="valueFactory">The factory to obtain the value to add.</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <returns>The added value.</returns>
    public static Result<TValue> GetOrAddResult<TKey, TValue>
    (
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Result<TValue>> valueFactory
    )
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var val))
        {
            return val;
        }

        var result = valueFactory(key);
        if (!result.IsSuccess)
        {
            return result;
        }

        dictionary.TryAdd(key, result.Entity);
        return result;
    }
}