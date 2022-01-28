//
//  FileReader.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using NosSmooth.Data.NOSFiles.Errors;
using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Readers;

/// <summary>
/// Reader of files.
/// </summary>
public class FileReader
{
    private readonly IReadOnlyList<IFileTypeReader> _typeReaders;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileReader"/> class.
    /// </summary>
    /// <param name="typeReaders">The readers of specific types.</param>
    public FileReader(IEnumerable<IFileTypeReader> typeReaders)
    {
        _typeReaders = typeReaders.ToArray();
    }

    /// <summary>
    /// Get a file type reader for the given file.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>A type reader or an error.</returns>
    public Result<IFileTypeReader> GetFileTypeReader(RawFile file)
    {
        foreach (var typeReader in _typeReaders)
        {
            if (typeReader.SupportsFile(file))
            {
                return Result<IFileTypeReader>.FromSuccess(typeReader);
            }
        }

        return new UnknownFileTypeError(file);
    }

    /// <summary>
    /// Read the given file.
    /// </summary>
    /// <param name="file">The raw file.</param>
    /// <typeparam name="TContent">The type of the content to assume.</typeparam>
    /// <returns>The content or an error.</returns>
    public Result<ReadFile<TContent>> Read<TContent>(RawFile file)
    {
        var fileReaderResult = GetFileTypeReader(file);
        if (!fileReaderResult.IsSuccess)
        {
            return Result<ReadFile<TContent>>.FromError(fileReaderResult);
        }

        var fileReader = fileReaderResult.Entity;
        var readResult = fileReader.Read(file);
        if (!readResult.IsSuccess)
        {
            return Result<ReadFile<TContent>>.FromError(readResult);
        }

        try
        {
            var content = (ReadFile<TContent>)readResult.Entity;
            return content;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Read a file from a filesystem path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <typeparam name="TContent">The type of the content to assume.</typeparam>
    /// <returns>The content or an error.</returns>
    public Result<ReadFile<TContent>> ReadFileSystemFile<TContent>(string path)
    {
        try
        {
            var readData = File.ReadAllBytes(path);
            return Read<TContent>(new RawFile(null, path, readData.Length, readData));
        }
        catch (Exception e)
        {
            return e;
        }
    }
}