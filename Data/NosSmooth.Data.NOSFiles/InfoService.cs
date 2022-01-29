//
//  InfoService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.NOSFiles.Parsers;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles;

/// <inheritdoc />
internal class InfoService : IInfoService
{
    private readonly NostaleData _nostaleData;

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoService"/> class.
    /// </summary>
    /// <param name="nostaleData">The parsed data.</param>
    public InfoService(NostaleData nostaleData)
    {
        _nostaleData = nostaleData;
    }

    /// <inheritdoc />
    public Result<IItemInfo> GetItemInfo(int vnum)
    {
        if (!_nostaleData.Items.ContainsKey(vnum))
        {
            return new NotFoundError($"Couldn't find item {vnum}");
        }

        return Result<IItemInfo>.FromSuccess(_nostaleData.Items[vnum]);
    }

    /// <inheritdoc />
    public Result<IMapInfo> GetMapInfo(int id)
    {
        if (!_nostaleData.Maps.ContainsKey(id))
        {
            return new NotFoundError($"Couldn't find item {id}");
        }

        return Result<IMapInfo>.FromSuccess(_nostaleData.Maps[id]);
    }

    /// <inheritdoc />
    public Result<IMonsterInfo> GetMonsterInfo(int vnum)
    {
        if (!_nostaleData.Monsters.ContainsKey(vnum))
        {
            return new NotFoundError($"Couldn't find item {vnum}");
        }

        return Result<IMonsterInfo>.FromSuccess(_nostaleData.Monsters[vnum]);
    }

    /// <inheritdoc />
    public Result<ISkillInfo> GetSkillInfo(int vnum)
    {
        if (!_nostaleData.Skills.ContainsKey(vnum))
        {
            return new NotFoundError($"Couldn't find item {vnum}");
        }

        return Result<ISkillInfo>.FromSuccess(_nostaleData.Skills[vnum]);
    }
}