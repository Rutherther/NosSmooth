//
//  ReadFile.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.Data.NOSFiles.Files;

/// <summary>
/// A file that has been already processed by a reader.
/// </summary>
/// <param name="Path">A path to the file.</param>
/// <param name="Content">The content read.</param>
/// <typeparam name="TContent">The type of the content read.</typeparam>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Upper case is standard.")]
public record struct ReadFile<TContent>
(
    string Path,
    TContent Content
);