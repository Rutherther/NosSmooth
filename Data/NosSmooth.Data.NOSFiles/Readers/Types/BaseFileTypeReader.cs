//
//  BaseFileTypeReader.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Readers.Types;

/// <inheritdoc />
public abstract class BaseFileTypeReader<TContent> : IFileTypeReader<TContent>
{
    /// <inheritdoc />
    public abstract Result<ReadFile<TContent>> ReadExact(RawFile file);

    /// <inheritdoc />
    public abstract bool SupportsFile(RawFile file);

    /// <inheritdoc />
    public Result<object> Read(RawFile file)
    {
        var readResult = ReadExact(file);
        if (!readResult.IsSuccess)
        {
            return Result<object>.FromError(readResult);
        }

        return Result<object>.FromSuccess(readResult.Entity);
    }
}