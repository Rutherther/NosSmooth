//
//  DatabaseInfoService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Abstractions.Infos;
using Remora.Results;

namespace NosSmooth.Data.Database;

/// <inheritdoc />
public class DatabaseInfoService : IInfoService
{
    private readonly IDbContextFactory<NostaleDataContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseInfoService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    public DatabaseInfoService(IDbContextFactory<NostaleDataContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc />
    public async Task<Result<IItemInfo>> GetItemInfoAsync(int vnum, CancellationToken ct = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var item = await context.Items.AsNoTracking().FirstOrDefaultAsync(x => x.VNum == vnum, ct);
        if (item is null)
        {
            return new NotFoundError($"Couldn't find item {vnum}");
        }

        return item;
    }

    /// <inheritdoc />
    public async Task<Result<IMapInfo>> GetMapInfoAsync(int id, CancellationToken ct = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var item = await context.Maps.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item is null)
        {
            return new NotFoundError($"Couldn't find map {id}");
        }

        return item;
    }

    /// <inheritdoc />
    public async Task<Result<IMonsterInfo>> GetMonsterInfoAsync(int vnum, CancellationToken ct = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var item = await context.Monsters.AsNoTracking().FirstOrDefaultAsync(x => x.VNum == vnum, ct);
        if (item is null)
        {
            return new NotFoundError($"Couldn't find monster {vnum}");
        }

        return item;
    }

    /// <inheritdoc />
    public async Task<Result<ISkillInfo>> GetSkillInfoAsync(int vnum, CancellationToken ct = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var item = await context.Skills.AsNoTracking().FirstOrDefaultAsync(x => x.VNum == vnum, ct);
        if (item is null)
        {
            return new NotFoundError($"Couldn't find skill {vnum}");
        }

        return item;
    }
}