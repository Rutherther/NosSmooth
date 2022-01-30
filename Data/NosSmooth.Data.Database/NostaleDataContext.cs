//
//  NostaleDataContext.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using NosSmooth.Data.Database.Data;

namespace NosSmooth.Data.Database;

/// <summary>
/// Database context with NosTale data.
/// </summary>
public class NostaleDataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleDataContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public NostaleDataContext(DbContextOptions<NostaleDataContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the translations set.
    /// </summary>
    public DbSet<Translation> Translations => Set<Translation>();

    /// <summary>
    /// Gets the items set.
    /// </summary>
    public DbSet<ItemInfo> Items => Set<ItemInfo>();

    /// <summary>
    /// Gets the maps set.
    /// </summary>
    public DbSet<MapInfo> Maps => Set<MapInfo>();

    /// <summary>
    /// Gets the monsters set.
    /// </summary>
    public DbSet<MonsterInfo> Monsters => Set<MonsterInfo>();

    /// <summary>
    /// Gets the skills set.
    /// </summary>
    public DbSet<SkillInfo> Skills => Set<SkillInfo>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemInfo>()
            .Property(x => x.Data)
            .HasConversion
            (
                x => string.Join("|", x),
                x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)
            );

        modelBuilder.Entity<Translation>().HasKey("Language", "Root", "Key");

        modelBuilder.Entity<ItemInfo>().Ignore(x => x.Name);
        modelBuilder.Entity<SkillInfo>().Ignore(x => x.Name);
        modelBuilder.Entity<MonsterInfo>().Ignore(x => x.Name);
        modelBuilder.Entity<MapInfo>().Ignore(x => x.Name);
        base.OnModelCreating(modelBuilder);
    }
}