//
//  DatItem.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.NOSFiles.Parsers.Dat;

/// <summary>
/// An item from a dat file obtained using <see cref="DatReader"/>.
/// </summary>
public struct DatItem
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<DatEntry>> _entries;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatItem"/> struct.
    /// </summary>
    /// <param name="entries">The entries of the item.</param>
    public DatItem(IReadOnlyDictionary<string, IReadOnlyList<DatEntry>> entries)
    {
        _entries = entries;
    }

    /// <summary>
    /// Gets the entry with the given name.
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    /// <returns>An entry, or null, if not found.</returns>
    public DatEntry? GetNullableEntry(string name)
    {
        return GetEntries(name)?.FirstOrDefault() ?? null;
    }

    /// <summary>
    /// Gets the entry with the given name.
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    /// <returns>An entry, or null, if not found.</returns>
    public DatEntry GetEntry(string name)
    {
        return GetEntries(name).First();
    }

    /// <summary>
    /// Gets the entry with the given name.
    /// </summary>
    /// <param name="name">The name of the entry.</param>
    /// <returns>An entry, or null, if not found.</returns>
    public IReadOnlyList<DatEntry> GetEntries(string name)
    {
        if (!_entries.ContainsKey(name))
        {
            return Array.Empty<DatEntry>();
        }

        return _entries[name];
    }
}