//
//  ContractError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Core.Contracts;

/// <summary>
/// An error from contract.
/// </summary>
/// <param name="Error">The error.</param>
/// <typeparam name="TError">The error.</typeparam>
public record ContractError<TError>(TError Error) : ResultError($"Contract has returned an error {Error}.");