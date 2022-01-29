//
//  RawFile.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Security.Cryptography;

namespace NosSmooth.Data.NOSFiles.Files;

/// <summary>
/// A file.
/// </summary>
/// <param name="FileType">The type of the file, if known.</param>
/// <param name="Path">The path to the file.</param>
/// <param name="Length">The length of the content.</param>
/// <param name="Content">The binary content of the file.</param>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Upper case is standard.")]
public record struct RawFile
(
    FileType? FileType,
    string Path,
    long Length,
    byte[] Content
);