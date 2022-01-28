//
//  FileType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.NOSFiles.Files;

/// <summary>
/// A type of a file.
/// </summary>
public enum FileType
{
    /// <summary>
    /// The file is a text file.
    /// </summary>
    Text,

    /// <summary>
    /// The file is a binary file with special meaning.
    /// </summary>
    Binary
}