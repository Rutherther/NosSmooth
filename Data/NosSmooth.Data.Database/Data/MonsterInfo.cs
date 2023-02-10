//
//  MonsterInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Database.Data;

/// <inheritdoc />
public class MonsterInfo : IMonsterInfo
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
            Name = new TranslatableString(TranslationRoot.Monster, value);
        }
    }

    /// <inheritdoc />
    public TranslatableString Name { get; set; }

    /// <inheritdoc />
    public ushort Level { get; set; }

    /// <inheritdoc />
    public short Range { get; set; }

    /// <inheritdoc />
    public short NoticeRange { get; set; }

    /// <inheritdoc />
    public int CastTime { get; set; }

    /// <inheritdoc />
    public bool Hostile { get; set; }
}