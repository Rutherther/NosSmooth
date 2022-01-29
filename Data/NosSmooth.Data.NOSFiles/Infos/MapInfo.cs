//
//  MapInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.NOSFiles.Infos;

/// <inheritdoc />
internal class MapInfo : IMapInfo
{
    private readonly byte[] _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapInfo"/> class.
    /// </summary>
    /// <param name="id">The VNum.</param>
    /// <param name="name">The name of the map.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="grid">The grid data.</param>
    public MapInfo(int id, TranslatableString name, short width, short height, byte[] grid)
    {
        Id = id;
        Name = name;
        Width = width;
        Height = height;
        _data = grid;
    }

    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public TranslatableString Name { get; }

    /// <inheritdoc />
    public short Width { get; }

    /// <inheritdoc />
    public short Height { get; }

    /// <inheritdoc />
    public byte GetData(short x, short y)
    {
        return _data[(y * Width) + x];
    }

    /// <inheritdoc />
    public bool IsWalkable(short x, short y)
    {
        var val = GetData(x, y);
        return val == 0 || val == 2 || (val >= 16 && val <= 19);
    }
}