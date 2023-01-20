//
//  DatabaseMigrator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using NosSmooth.Data.Abstractions;
using NosSmooth.Data.Database.Data;
using Remora.Results;

namespace NosSmooth.Data.Database;

/// <summary>
/// Migrates Nostale data into sqlite database.
/// </summary>
public class DatabaseMigrator
{
    private readonly IDbContextFactory<NostaleDataContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseMigrator"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    public DatabaseMigrator(IDbContextFactory<NostaleDataContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Migrates the data into the database.
    /// </summary>
    /// <param name="data">The NosTale data.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public async Task<Result> Migrate(NostaleData data, CancellationToken ct = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var itemsResult = await MigrateItems(context, data);
        if (!itemsResult.IsSuccess)
        {
            return itemsResult;
        }

        var skillsResult = await MigrateSkills(context, data);
        if (!skillsResult.IsSuccess)
        {
            return skillsResult;
        }

        var monstersResult = await MigrateMonsters(context, data);
        if (!monstersResult.IsSuccess)
        {
            return monstersResult;
        }

        var mapsResult = await MigrateMaps(context, data);
        if (!mapsResult.IsSuccess)
        {
            return mapsResult;
        }

        var translationsResult = await MigrateTranslations(context, data);
        if (!translationsResult.IsSuccess)
        {
            return translationsResult;
        }

        await context.Database.EnsureDeletedAsync(ct);
        await context.Database.EnsureCreatedAsync(ct);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    private Task<Result> MigrateTranslations(NostaleDataContext context, NostaleData data)
    {
        foreach (var languageTranslation in data.Translations)
        {
            foreach (var rootTranslations in languageTranslation.Value)
            {
                foreach (var translations in rootTranslations.Value)
                {
                    var translation = new Translation
                    {
                        Key = translations.Key,
                        Root = rootTranslations.Key,
                        Language = languageTranslation.Key,
                        Translated = translations.Value
                    };
                    context.Add(translation);
                }
            }
        }

        return Task.FromResult(Result.FromSuccess());
    }

    private Task<Result> MigrateItems(NostaleDataContext dbContext, NostaleData data)
    {
        foreach (var item in data.Items.Values)
        {
            var itemInfo = new ItemInfo
            {
                BagType = item.BagType,
                Data = item.Data,
                EquipmentSlot = item.EquipmentSlot,
                NameKey = item.Name.Key,
                SubType = item.SubType,
                Type = item.Type,
                VNum = item.VNum
            };
            dbContext.Add(itemInfo);
        }

        return Task.FromResult(Result.FromSuccess());
    }

    private Task<Result> MigrateSkills(NostaleDataContext dbContext, NostaleData data)
    {
        foreach (var skill in data.Skills.Values)
        {
            var skillInfo = new SkillInfo
            {
                CastId = skill.CastId,
                CastTime = skill.CastTime,
                Cooldown = skill.Cooldown,
                HitType = skill.HitType,
                MpCost = skill.MpCost,
                NameKey = skill.Name.Key,
                Range = skill.Range,
                SkillType = skill.SkillType,
                TargetType = skill.TargetType,
                VNum = skill.VNum,
                ZoneRange = skill.ZoneRange,
                AttackType = skill.AttackType,
                Element = skill.Element,
                UsesSecondaryWeapon = skill.UsesSecondaryWeapon,
                DashSpeed = skill.DashSpeed,
                ItemVNum = skill.ItemVNum,
                MorphOrUpgrade = skill.MorphOrUpgrade,
                Name = skill.Name,
                SpecialCost = skill.SpecialCost,
                Upgrade = skill.Upgrade
            };
            dbContext.Add(skillInfo);
        }

        return Task.FromResult(Result.FromSuccess());
    }

    private Task<Result> MigrateMonsters(NostaleDataContext dbContext, NostaleData data)
    {
        foreach (var monster in data.Monsters.Values)
        {
            var monsterInfo = new MonsterInfo
            {
                VNum = monster.VNum,
                NameKey = monster.Name.Key,
                Level = monster.Level
            };
            dbContext.Add(monsterInfo);
        }

        return Task.FromResult(Result.FromSuccess());
    }

    private Task<Result> MigrateMaps(NostaleDataContext dbContext, NostaleData data)
    {
        foreach (var map in data.Maps.Values)
        {
            var grid = new byte[map.Height * map.Width];
            for (short y = 0; y < map.Height; y++)
            {
                for (short x = 0; x < map.Width; x++)
                {
                    grid[(y * map.Width) + x] = map.GetData(x, y);
                }
            }

            var mapInfo = new MapInfo
            {
                Height = map.Height,
                Width = map.Width,
                NameKey = map.Name.Key,
                Id = map.Id,
                Grid = grid
            };
            dbContext.Add(mapInfo);
        }

        return Task.FromResult(Result.FromSuccess());
    }
}