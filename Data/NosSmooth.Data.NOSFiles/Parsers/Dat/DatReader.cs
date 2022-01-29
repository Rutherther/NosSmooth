//
//  DatReader.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Parsers.Dat;

/// <summary>
/// Reader of .dat files.
/// </summary>
public class DatReader
{
    private readonly RawFile _file;
    private IReadOnlyList<string> _lines;
    private int _currentLine;
    private string _separator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatReader"/> class.
    /// </summary>
    /// <param name="file">The file to read.</param>
    public DatReader(RawFile file)
    {
        _lines = Encoding.ASCII.GetString(file.Content).Split('\n').ToArray();
        _currentLine = 0;
        _file = file;
        _separator = "VNUM";
    }

    /// <summary>
    /// Gets whether the reader has reached the end.
    /// </summary>
    public bool ReachedEnd => _currentLine + 1 >= _lines.Count;

    /// <summary>
    /// Sets the separator of a new item.
    /// </summary>
    /// <param name="separator">The separator of new item.</param>
    public void SetSeparatorField(string separator)
    {
        _separator = separator;
    }

    /// <summary>
    /// Read next item.
    /// </summary>
    /// <param name="item">The read item.</param>
    /// <returns>Whether an item was read.</returns>
    public bool ReadItem([NotNullWhen(true)] out DatItem? item)
    {
        if (ReachedEnd)
        {
            item = null;
            return false;
        }

        bool readFirstItem = _currentLine > 0;
        int startLine = _currentLine;

        while (!ReachedEnd && !_lines[_currentLine].StartsWith(_separator))
        {
            _currentLine++;
        }

        if (!readFirstItem)
        {
            return ReadItem(out item);
        }

        var dictionary = new Dictionary<string, IReadOnlyList<DatEntry>>();
        for (int i = startLine; i < _currentLine; i++)
        {
            var line = _lines[i];
            var splitted = line.Split('\t');
            var key = splitted[0];
            var entry = new DatEntry(key, splitted);
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<DatEntry>());
            }

            ((List<DatEntry>)dictionary[key]).Add(entry);
        }

        item = new DatItem(dictionary);
        return true;
    }
}