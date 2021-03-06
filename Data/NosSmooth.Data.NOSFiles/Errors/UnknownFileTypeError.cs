//
//  UnknownFileTypeError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Errors;

/// <summary>
/// The file type of the file does not have a reader registered.
/// </summary>
/// <param name="file">The file.</param>
public record UnknownFileTypeError(RawFile file) : NotFoundError($"Could not find reader for the given file {file.Path}.");