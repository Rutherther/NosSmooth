//
//  ExtractNosFileCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Text;
using NosSmooth.Data.NOSFiles.Files;
using NosSmooth.Data.NOSFiles.Readers;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace NosSmooth.Data.CLI.Commands;

/// <summary>
/// A command to extract files from a NosTale archive.
/// </summary>
public class ExtractNosFileCommand : CommandGroup
{
    private readonly FileReader _reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractNosFileCommand"/> class.
    /// </summary>
    /// <param name="reader">The file reader.</param>
    public ExtractNosFileCommand(FileReader reader)
    {
        _reader = reader;
    }

    /// <summary>
    /// Handles extract command.
    /// </summary>
    /// <param name="inputFile">The input nos archive.</param>
    /// <param name="outputDirectory">The output directory to put the extracted files to.</param>
    /// <returns>A result that may or may not have succeeded..</returns>
    [Command("extract")]
    public async Task<Result> HandleExtract
    (
        string inputFile,
        [Option('o', "option")]
        string outputDirectory = "out"
    )
    {
        Directory.CreateDirectory(outputDirectory);

        var readResult = _reader.ReadFileSystemFile<FileArchive>(inputFile);
        if (!readResult.IsSuccess)
        {
            return Result.FromError(readResult);
        }

        foreach (var file in readResult.Entity.Content.Files)
        {
            var outputPath = Path.Combine(outputDirectory, file.Path);
            await File.WriteAllBytesAsync(outputPath, file.Content, CancellationToken);
        }

        return Result.FromSuccess();
    }
}