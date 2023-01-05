//
//  ItemInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Database.Data;

/// <inheritdoc />
public class ItemInfo : IItemInfo
{
    private string _nameKey = null!;

    /// <inheritdoc />
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public int VNum { get; set; }

    /// <summary>
    /// The name translation key.
    /// </summary>
    public string NameKey
    {
        get => _nameKey;
        set
        {
            _nameKey = value;
            Name = new TranslatableString(TranslationRoot.Item, value);
        }
    }

    /// <inheritdoc />
    public TranslatableString Name { get; set; }

    /// <inheritdoc />
    public ItemType Type { get; set; }

    /// <inheritdoc />
    public int SubType { get; set; }

    /// <inheritdoc />
    public EquipmentSlot? EquipmentSlot { get; set; }

    /// <inheritdoc />
    public BagType BagType { get; set; }

    /// <inheritdoc/>
    public string[] Data { get; set; } = null!;
}