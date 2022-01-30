//
//  ItemParser.cs
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
public class ItemParser : IInfoParser<IItemInfo>
{
    /// <inheritdoc />
    public Result<Dictionary<int, IItemInfo>> Parse(NostaleFiles files)
    {
        var itemDatResult = files.DatFiles.FindFile("Item.dat");
        if (!itemDatResult.IsSuccess)
        {
            return Result<Dictionary<int, IItemInfo>>.FromError(itemDatResult);
        }
        var reader = new DatReader(itemDatResult.Entity);
        var result = new Dictionary<int, IItemInfo>();

        while (reader.ReadItem(out var itemNullable))
        {
            var item = itemNullable.Value;
            var indexEntry = item.GetEntry("INDEX");

            var vnum = item.GetEntry("VNUM").Read<int>(1);
            var nameKey = item.GetEntry("NAME").Read<string>(1);
            result.Add
            (
                vnum,
                new ItemInfo
                (
                    vnum,
                    new TranslatableString(TranslationRoot.Item, nameKey),
                    indexEntry.Read<int>(2),
                    indexEntry.Read<int>(3),
                    (BagType)indexEntry.Read<int>(1),
                    item.GetEntry("DATA").GetValues()
                )
            );
        }

        return result;
    }

    private record ItemInfo
        (
            int VNum,
            TranslatableString Name,
            int Type,
            int SubType,
            BagType BagType,
            string[] Data
        )
        : IItemInfo;
}