//
//  RawFile.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Security.Cryptography;

namespace NosSmooth.Data.NOSFiles.Files;

[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Upper case is standard.")]
public record struct RawFile
(
    FileType? FileType,
    string Path,
    long Length,
    byte[] Content
);