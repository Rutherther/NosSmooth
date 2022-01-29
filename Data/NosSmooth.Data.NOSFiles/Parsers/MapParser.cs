//
//  MapParser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Infos;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Parsers;

/// <inheritdoc />
public class MapParser : IInfoParser<IMapInfo>
{
    /// <inheritdoc />
    public Result<Dictionary<int, IMapInfo>> Parse(NostaleFiles files)
    {
        var mapDatResult = files.DatFiles.FindFile("MapIDData.dat");
        if (!mapDatResult.IsSuccess)
        {
            return Result<Dictionary<int, IMapInfo>>.FromError(mapDatResult);
        }
        var mapGridsArchive = files.MapGridsFiles;
        var mapDatContent = Encoding.ASCII.GetString(mapDatResult.Entity.Content).Split('\r', '\n');

        var mapNames = new Dictionary<int, TranslatableString>();
        foreach (var line in mapDatContent)
        {
            var splitted = line.Split(' ', '\t');
            if (splitted.Length != 5)
            {
                continue;
            }

            var first = int.Parse(splitted[0]);
            var second = int.Parse(splitted[1]);
            if (first == second)
            {
                mapNames[first] = new TranslatableString(TranslationRoot.Map, splitted.Last());
            }

            for (int i = first; i < second; i++)
            {
                mapNames[i] = new TranslatableString(TranslationRoot.Map, splitted.Last());
            }
        }

        var result = new Dictionary<int, IMapInfo>();
        foreach (var file in mapGridsArchive.Files)
        {
            var id = int.Parse(file.Path);
            var grid = file.Content;
            result[id] = new MapInfo
            (
                id,
                mapNames.GetValueOrDefault(id, new TranslatableString(TranslationRoot.Map, "Map")),
                BitConverter.ToInt16(grid.Take(2).ToArray()),
                BitConverter.ToInt16(grid.Skip(2).Take(2).ToArray()),
                grid.Skip(4).ToArray()
            );
        }

        return result;
    }
}