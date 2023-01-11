//
//  IContract.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Contracts.Responders;
using Remora.Results;

namespace NosSmooth.Core.Contracts;

/// <summary>
/// A contract, used for executing an operation with feedback.
/// </summary>
/// <remarks>
/// Do not use this type directly, use the generic one instead.
/// </remarks>
public interface IContract
{
    /// <summary>
    /// Gets whether this contract
    /// is registered to the contractor.
    /// </summary>
    public bool IsRegistered { get; }

    /// <summary>
    /// Register this contract into contractor.
    /// </summary>
    /// <remarks>
    /// The contract will receive data from the contractor,
    /// using CheckDataAsync method. This way there may be a
    /// feedback coming back to the contract.
    /// </remarks>
    public void Register();

    /// <summary>
    /// Unregister this contract from contractor.
    /// </summary>
    public void Unregister();

    /// <summary>
    /// Update the contract with the given received data.
    /// </summary>
    /// <remarks>
    /// Called from <see cref="ContractPacketResponder"/>
    /// or similar. Used for updating the state.
    /// The contract looks for actions that trigger updates
    /// and in case it matches the <paramref name="data"/>,
    /// the state is switched.
    /// </remarks>
    /// <param name="data">The data that were received.</param>
    /// <typeparam name="TAny">The type of the data.</typeparam>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>The result that may or may not have succeeded.</returns>
    public Task<Result<ContractUpdateResponse>> Update<TAny>(TAny data, CancellationToken ct = default);

    /// <summary>
    /// Executes the contract without registering it,
    /// running only the initial operation.
    /// </summary>
    /// <remarks>
    /// For example, to use skill, create a contract for
    /// using a skill and call this method.
    /// If you want to wait for response from the server,
    /// use <see cref="WaitForAsync"/> instead.
    /// That will register the contract and wait for response.
    /// </remarks>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>The result that may or may not have succeeded.</returns>
    public Task<Result> OnlyExecuteAsync(CancellationToken ct = default);
}

/// <summary>
/// A contract, used for executing an operation with feedback.
/// </summary>
/// <remarks>
/// Could be used for operations that may end successfully or fail
/// after some time, with response from the server.
///
/// Look at <see cref="ContractBuilder"/> for example usage.
/// </remarks>
/// <typeparam name="TData">The data returned by the contract in case of success.</typeparam>
/// <typeparam name="TState">Type containing the states of the contract.</typeparam>
public interface IContract<TData, TState> : IContract
    where TData : notnull
    where TState : IComparable
{
    /// <summary>
    /// Gets the current state of the contract.
    /// </summary>
    /// <remarks>
    /// To wait for any state, see <see cref="WaitForAsync"/>.
    /// </remarks>
    public TState CurrentState { get; }

    /// <summary>
    /// Gets the data of the contract obtained from packets/.
    /// </summary>
    /// <remarks>
    /// This won't be filled in case the contract
    /// is not registered.
    /// </remarks>
    public TData? Data { get; }

    /// <summary>
    /// Register to contractor and wait for the given state.
    /// Execute the initial action.
    /// </summary>
    /// <param name="state">The state to wait for.</param>
    /// <param name="unregisterAfter">Whether to unregister the contract from the contractor after the state is reached. The contract won't be updated anymore.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>The data of the contract or an error.</returns>
    /// <exception cref="InvalidOperationError">Thrown in case the given state cannot fill the data.</exception>
    public Task<Result<TData>> WaitForAsync(TState state, bool unregisterAfter = true, CancellationToken ct = default);
}