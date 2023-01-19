//
//  ContractMultipleStates.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Tests.Contracts;

/// <summary>
/// Extends DefaultStates to have more states to test.
/// </summary>
public enum ContractMultipleStates
{
    /// <summary>
    /// Initial state.
    /// </summary>
    None,

    /// <summary>
    /// Contract executed, request issued.
    /// </summary>
    Requested,

    /// <summary>
    /// A response was obtained.
    /// </summary>
    ResponseObtained,

    /// <summary>
    /// Something else happening after obtaining first response.
    /// </summary>
    AfterResponseObtained,
}