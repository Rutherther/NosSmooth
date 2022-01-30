//
//  MigrateDatabaseCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Language;
using NosSmooth.Data.Database;
using NosSmooth.Data.NOSFiles;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace NosSmooth.Data.CLI.Commands;

/// <summary>
/// Create a database from nos files.
/// </summary>
public class MigrateDatabaseCommand : CommandGroup
{
    private readonly DatabaseMigrator _migrator;
    private readonly NostaleDataParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrateDatabaseCommand"/> class.
    /// </summary>
    /// <param name="migrator">The database migrator.</param>
    /// <param name="parser">The data parser.</param>
    public MigrateDatabaseCommand(DatabaseMigrator migrator, NostaleDataParser parser)
    {
        _migrator = migrator;
        _parser = parser;
    }

    /// <summary>
    /// Migrate the database using nos files.
    /// </summary>
    /// <param name="nostaleDataPath">The directory with nostale data files.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Command("migrate")]
    public async Task<Result> HandleMigrate([Greedy] string nostaleDataPath)
    {
        var parsingResult = _parser.ParseFiles
        (
            nostaleDataPath,
            Language.Cz,
            Language.De,
            Language.Uk,
            Language.Es,
            Language.Fr,
            Language.It,
            Language.Pl,
            Language.Ru,
            Language.Tr
        );
        if (!parsingResult.IsSuccess)
        {
            return Result.FromError(parsingResult);
        }

        return await _migrator.Migrate(parsingResult.Entity);
    }
}