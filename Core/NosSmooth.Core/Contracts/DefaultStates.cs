//
//  DefaultStates.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Contracts;

/// <summary>
/// Default states used for contracts.
/// </summary>
public enum DefaultStates
{
    /// <summary>
    /// Contract has not been executed yet.
    /// </summary>
    None,

    /// <summary>
    /// The contract has requested a response, waiting for it.
    /// </summary>
    Requested,

    /// <summary>
    /// The response was obtained.
    /// </summary>
    ResponseObtained
}