//
//  FileArchive.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Files;

/// <summary>
/// An archive of files.
/// </summary>
/// <param name="Files">The files in the archive.</param>
public record FileArchive(IReadOnlyList<RawFile> Files)
{
    /// <summary>
    /// Try to find the given file.
    /// </summary>
    /// <param name="name">The name of the file.</param>
    /// <returns>A file or an error.</returns>
    public Result<RawFile> FindFile(string name)
    {
        var foundFile = Files.OfType<RawFile?>().FirstOrDefault
            (x => Path.GetFileName(((RawFile)x!).Path) == name, null);
        if (foundFile is null)
        {
            return new NotFoundError($"Could not find file {name} in archive.");
        }

        return (RawFile)foundFile;
    }
}