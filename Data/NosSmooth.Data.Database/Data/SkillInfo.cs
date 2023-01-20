//
//  SkillInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;

namespace NosSmooth.Data.Database.Data;

/// <inheritdoc />
public class SkillInfo : ISkillInfo
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
            Name = new TranslatableString(TranslationRoot.Skill, value);
        }
    }

    /// <inheritdoc />
    public TranslatableString Name { get; set; }

    /// <inheritdoc />
    public short Range { get; set; }

    /// <inheritdoc />
    public short ZoneRange { get; set; }

    /// <inheritdoc />
    public int CastTime { get; set; }

    /// <inheritdoc />
    public int Cooldown { get; set; }

    /// <inheritdoc />
    public SkillType SkillType { get; set; }

    /// <inheritdoc />
    public AttackType AttackType { get; set; }

    /// <inheritdoc />
    public Element Element { get; set; }

    /// <inheritdoc />
    public bool UsesSecondaryWeapon { get; set; }

    /// <inheritdoc />
    public int MpCost { get; set; }

    /// <inheritdoc />
    public short CastId { get; set; }

    /// <inheritdoc />
    public TargetType TargetType { get; set; }

    /// <inheritdoc />
    public HitType HitType { get; set; }
}