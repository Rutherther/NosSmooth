//
//  MonsterParser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Parsers.Dat;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Parsers;

/// <inheritdoc />
public class MonsterParser : IInfoParser<IMonsterInfo>
{
    /// <inheritdoc />
    public Result<Dictionary<int, IMonsterInfo>> Parse(NostaleFiles files)
    {
        var monsterDatResult = files.DatFiles.FindFile("monster.dat");
        if (!monsterDatResult.IsSuccess)
        {
            return Result<Dictionary<int, IMonsterInfo>>.FromError(monsterDatResult);
        }
        var reader = new DatReader(monsterDatResult.Entity);
        var result = new Dictionary<int, IMonsterInfo>();

        while (reader.ReadItem(out var itemNullable))
        {
            var item = itemNullable.Value;

            var vnum = item.GetEntry("VNUM").Read<int>(1);
            var nameKey = item.GetEntry("NAME").Read<string>(1);
            result.Add
            (
                vnum,
                new MonsterInfo
                (
                    vnum,
                    new TranslatableString(TranslationRoot.Monster, nameKey),
                    item.GetEntry("LEVEL").Read<int>(1)
                )
            );
        }

        return result;
    }

    private record MonsterInfo(int VNum, TranslatableString Name, int Level) : IMonsterInfo;
}