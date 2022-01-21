//
//  IVNumInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Data.Abstractions.Infos;

/// <summary>
/// A NosTale info with a vnum key.
/// </summary>
public interface IVNumInfo
{
    /// <summary>
    /// Gets the VNum of the info entry.
    /// </summary>
    public long VNum { get; }
}