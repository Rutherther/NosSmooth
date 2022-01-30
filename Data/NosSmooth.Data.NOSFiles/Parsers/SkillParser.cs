//
//  SkillParser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Parsers.Dat;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Parsers;

/// <summary>
/// Parses Skill.dat.
/// </summary>
public class SkillParser : IInfoParser<ISkillInfo>
{
    /// <inheritdoc />
    public Result<Dictionary<int, ISkillInfo>> Parse(NostaleFiles files)
    {
        var skillDatResult = files.DatFiles.FindFile("Skill.dat");
        if (!skillDatResult.IsSuccess)
        {
            return Result<Dictionary<int, ISkillInfo>>.FromError(skillDatResult);
        }
        var reader = new DatReader(skillDatResult.Entity);
        var result = new Dictionary<int, ISkillInfo>();

        while (reader.ReadItem(out var itemNullable))
        {
            var item = itemNullable.Value;
            var typeEntry = item.GetEntry("TYPE");
            var targetEntry = item.GetEntry("TARGET");
            var dataEntry = item.GetEntry("DATA");

            var vnum = item.GetEntry("VNUM").Read<int>(1);
            var nameKey = item.GetEntry("NAME").Read<string>(1);
            result.Add
            (
                vnum,
                new SkillInfo
                (
                    vnum,
                    new TranslatableString(TranslationRoot.Skill, nameKey),
                    targetEntry.Read<short>(3),
                    targetEntry.Read<short>(4),
                    dataEntry.Read<int>(5),
                    dataEntry.Read<int>(6),
                    (SkillType)typeEntry.Read<int>(1),
                    dataEntry.Read<int>(7),
                    typeEntry.Read<short>(2),
                    (TargetType)targetEntry.Read<int>(1),
                    (HitType)targetEntry.Read<int>(2)
                )
            );
        }

        return result;
    }

    private record SkillInfo
    (
        int VNum,
        TranslatableString Name,
        short Range,
        short ZoneRange,
        int CastTime,
        int Cooldown,
        SkillType SkillType,
        int MpCost,
        short CastId,
        TargetType TargetType,
        HitType HitType
    ) : ISkillInfo;
}