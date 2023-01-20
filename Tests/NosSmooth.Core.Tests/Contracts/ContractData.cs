//
//  ContractData.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Core.Tests.Contracts;

/// <summary>
/// Data for updating a contract.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public class ContractData<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContractData{T}"/> class.
    /// </summary>
    /// <param name="data">The data to pass.</param>
    public ContractData(T data)
    {
        Data = data;
    }

    /// <summary>
    /// Gets the data.
    /// </summary>
    public T Data { get; }
}