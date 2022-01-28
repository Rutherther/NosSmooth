//
//  IFileTypeReader.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml;
using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Readers;

/// <summary>
/// Reader of a particular file type.
/// </summary>
public interface IFileTypeReader
{
    /// <summary>
    /// Checks whether the given raw file is supported by this reader.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns>Whether the file can be read by this reader.</returns>
    public bool SupportsFile(RawFile file);

    /// <summary>
    /// Read the given file contents.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <returns>Contents of the file or an error.</returns>
    public Result<object> Read(RawFile file);
}

/// <summary>
/// Reader of a particular file type.
/// </summary>
/// <typeparam name="TContent">The content of the file.</typeparam>
public interface IFileTypeReader<TContent> : IFileTypeReader
{
    /// <summary>
    /// Read the given file contents.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <returns>Contents of the file or an error.</returns>
    public Result<ReadFile<TContent>> ReadExact(RawFile file);
}