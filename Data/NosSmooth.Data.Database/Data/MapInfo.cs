//
//  MapInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Database.Data;

/// <inheritdoc />
public class MapInfo : IMapInfo
{
    private string _nameKey = null!;

    /// <inheritdoc />
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The name translation key.
    /// </summary>
    public string NameKey
    {
        get => _nameKey;
        set
        {
            _nameKey = value;
            Name = new TranslatableString(TranslationRoot.Map, value);
        }
    }

    /// <inheritdoc />
    public TranslatableString Name { get; set; }

    /// <inheritdoc />
    public short Width { get; set; }

    /// <inheritdoc />
    public short Height { get; set; }

    /// <summary>
    /// Gets or sets the grid data of the map.
    /// </summary>
    public byte[] Grid { get; set; } = null!;

    /// <inheritdoc />
    public byte GetData(short x, short y)
    {
        return Grid[(y * Width) + x];
    }

    /// <inheritdoc />
    public bool IsWalkable(short x, short y)
    {
        var val = GetData(x, y);
        return val == 0 || val == 2 || (val >= 16 && val <= 19);
    }
}