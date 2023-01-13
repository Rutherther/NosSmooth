//
//  ContractUpdateResponse.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Contracts;

/// <summary>
/// A response to Contract.Update.
/// </summary>
public enum ContractUpdateResponse
{
    /// <summary>
    /// The contract is not interested in the given data.
    /// </summary>
    NotInterested,

    /// <summary>
    /// The contract is interested in the given data,
    /// the data was used to update the contract and
    /// the contract wants to stay registered.
    /// </summary>
    Interested,

    /// <summary>
    /// The contract is interested in the given data,
    /// the data was used to update the contract and
    /// the contract wants to be unregistered from the contractor.
    /// </summary>
    InterestedAndUnregister
}