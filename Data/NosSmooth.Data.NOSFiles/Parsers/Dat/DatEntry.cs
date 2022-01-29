//
//  DatEntry.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.NOSFiles.Parsers.Dat;

/// <summary>
/// An entry for a <see cref="DatItem"/>.
/// </summary>
public struct DatEntry
{
    private readonly IReadOnlyList<string> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatEntry"/> struct.
    /// </summary>
    /// <param name="key">The key of the entry.</param>
    /// <param name="data">The data of the entry.</param>
    public DatEntry(string key, IReadOnlyList<string> data)
    {
        Key = key;
        _data = data;
    }

    /// <summary>
    /// Gets the key of the entry.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Read a value on the given index.
    /// </summary>
    /// <param name="index">The index to read at.</param>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>Read value.</returns>
    public T Read<T>(int index)
    {
        return (T)Convert.ChangeType(_data[index], typeof(T));
    }

    /// <summary>
    /// Get the values of the current entry.
    /// </summary>
    /// <remarks>
    /// Skips the header.
    /// </remarks>
    /// <returns>An array with the values.</returns>
    public string[] GetValues()
    {
        return _data.Skip(1).ToArray();
    }
}