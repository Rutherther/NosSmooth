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
    public Task<Result<IItemInfo>> GetItemInfoAsync(int vnum, CancellationToken ct = default)
    {
        if (!_nostaleData.Items.ContainsKey(vnum))
        {
            return Task.FromResult(Result<IItemInfo>.FromError(new NotFoundError($"Couldn't find item {vnum}")));
        }

        return Task.FromResult(Result<IItemInfo>.FromSuccess(_nostaleData.Items[vnum]));
    }

    /// <inheritdoc />
    public Task<Result<IMapInfo>> GetMapInfoAsync(int id, CancellationToken ct = default)
    {
        if (!_nostaleData.Maps.ContainsKey(id))
        {
            return Task.FromResult(Result<IMapInfo>.FromError(new NotFoundError($"Couldn't find map {id}")));
        }

        return Task.FromResult(Result<IMapInfo>.FromSuccess(_nostaleData.Maps[id]));
    }

    /// <inheritdoc />
    public Task<Result<IMonsterInfo>> GetMonsterInfoAsync(int vnum, CancellationToken ct = default)
    {
        if (!_nostaleData.Monsters.ContainsKey(vnum))
        {
            return Task.FromResult(Result<IMonsterInfo>.FromError(new NotFoundError($"Couldn't find monster {vnum}")));
        }

        return Task.FromResult(Result<IMonsterInfo>.FromSuccess(_nostaleData.Monsters[vnum]));
    }

    /// <inheritdoc />
    public Task<Result<ISkillInfo>> GetSkillInfoAsync(int vnum, CancellationToken ct = default)
    {
        if (!_nostaleData.Skills.ContainsKey(vnum))
        {
            return Task.FromResult(Result<ISkillInfo>.FromError(new NotFoundError($"Couldn't find skill {vnum}")));
        }

        return Task.FromResult(Result<ISkillInfo>.FromSuccess(_nostaleData.Skills[vnum]));
    }
}