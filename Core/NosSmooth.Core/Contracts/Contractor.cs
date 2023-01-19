//
//  Contractor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Contracts.Responders;
using Remora.Results;

namespace NosSmooth.Core.Contracts;

/// <summary>
/// A class holding <see cref="IContract"/>s,
/// updates the contracts.
/// </summary>
public class Contractor : IEnumerable<IContract>
{
    /// <summary>
    /// Maximum time a contract may be registered for.
    /// </summary>
    public static readonly TimeSpan Timeout = new TimeSpan(0, 5, 0);

    private readonly List<ContractInfo> _contracts;
    private readonly ReaderWriterLockSlim _semaphore;

    /// <summary>
    /// Initializes a new instance of the <see cref="Contractor"/> class.
    /// </summary>
    public Contractor()
    {
        _semaphore = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        _contracts = new List<ContractInfo>();
    }

    /// <summary>
    /// Register the given contract to receive feedback for it.
    /// </summary>
    /// <param name="contract">The contract to register.</param>
    public void Register(IContract contract)
    {
        try
        {
            _semaphore.EnterWriteLock();
            _contracts.Add(new ContractInfo(contract, DateTime.Now));
        }
        finally
        {
            _semaphore.ExitWriteLock();
        }
    }

    /// <summary>
    /// Unregister the given contract, no more info will be received.
    /// </summary>
    /// <param name="contract">The contract.</param>
    public void Unregister(IContract contract)
    {
        try
        {
            _semaphore.EnterWriteLock();
            _contracts.RemoveAll(x => x.contract == contract);
        }
        finally
        {
            _semaphore.ExitWriteLock();
        }
    }

    /// <summary>
    /// Update all of the contracts with the given data.
    /// </summary>
    /// <remarks>
    /// Called from <see cref="ContractPacketResponder"/>
    /// or similar. Used for updating the state.
    /// The contracts look for actions that trigger updates
    /// and in case it matches the <paramref name="data"/>,
    /// the state is switched.
    /// </remarks>
    /// <param name="data">The data that were received.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <returns>The result that may or may not have succeeded.</returns>
    public async Task<Result> Update<TData>(TData data, CancellationToken ct = default)
        where TData : notnull
    {
        var errors = new List<IResult>();
        var toRemove = new List<ContractInfo>();
        ContractInfo[] contracts;

        try
        {
            _semaphore.EnterReadLock();
            contracts = _contracts.ToArray();
        }
        finally
        {
            _semaphore.ExitReadLock();
        }

        foreach (var info in contracts)
        {
            if (DateTime.Now - info.addedAt > Timeout)
            {
                errors.Add
                (
                    (Result)new GenericError
                    (
                        $"A contract {info.contract} has been registered for too long and was unregistered automatically."
                    )
                );
                continue;
            }

            var result = await info.contract.Update(data, ct);
            if (!result.IsDefined(out var response))
            {
                errors.Add(result);
            }

            if (response == ContractUpdateResponse.InterestedAndUnregister)
            {
                toRemove.Add(info);
            }
        }

        if (toRemove.Count > 0)
        {
            try
            {
                _semaphore.EnterWriteLock();
                foreach (var contract in toRemove)
                {
                    _contracts.Remove(contract);
                }
            }
            finally
            {
                _semaphore.ExitWriteLock();
            }
        }

        return errors.Count switch
        {
            0 => Result.FromSuccess(),
            1 => (Result)errors[0],
            _ => new AggregateError(errors)
        };
    }

    /// <inheritdoc />
    public IEnumerator<IContract> GetEnumerator()
        => _contracts.Select(x => x.contract).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private record struct ContractInfo(IContract contract, DateTime addedAt);
}