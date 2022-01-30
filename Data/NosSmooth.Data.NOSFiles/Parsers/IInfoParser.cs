//
//  IInfoParser.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.NOSFiles.Files;
using Remora.Results;

namespace NosSmooth.Data.NOSFiles.Parsers;

/// <summary>
/// A parser of info.
/// </summary>
/// <typeparam name="TType">The type of the info to parse.</typeparam>
public interface IInfoParser<TType>
{
    /// <summary>
    /// Parse the data from the given file.
    /// </summary>
    /// <param name="files">The NosTale files.</param>
    /// <returns>The list of the parsed entries.</returns>
    public Result<Dictionary<int, TType>> Parse(NostaleFiles files);
}