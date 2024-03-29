//
//  DefaultContract.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Contracts;

/// <summary>
/// A generic implementation of contract
/// supporting any data.
/// </summary>
/// <typeparam name="TData">The data type.</typeparam>
/// <typeparam name="TState">The states.</typeparam>
/// <typeparam name="TError">The errors that may be returned.</typeparam>
public class DefaultContract<TData, TState, TError> : IContract<TData, TState>
    where TState : struct, IComparable
    where TError : struct
    where TData : notnull
{
    /// <summary>
    /// An action to execute when a state is reached.
    /// </summary>
    /// <param name="data">The data that led to the state.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>The result that may or may not have succeeded.</returns>
    public delegate Task<Result<(TError? Error, TState? NextState)>> StateActionAsync
        (object? data, CancellationToken ct);

    /// <summary>
    /// An action to execute when a state that may fill the data is reached.
    /// Returns the data to fill.
    /// </summary>
    /// <param name="data">The data that led to the state.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>The result that may or may not have succeeded.</returns>
    public delegate Task<Result<TData>> FillDataAsync(object data, CancellationToken ct);

    private readonly SemaphoreSlim _semaphore;

    private readonly IDictionary<TState, (TimeSpan, TState?, TError?)> _timeouts;
    private readonly IDictionary<TState, StateActionAsync> _actions;
    private readonly Contractor _contractor;
    private readonly TState _defaultState;

    private readonly TState _fillAtState;
    private readonly FillDataAsync _fillData;

    private TError? _error;
    private Result? _resultError;

    private TState? _waitingFor;
    private bool _unregisterAtWaitingFor;
    private CancellationTokenSource? _waitCancellationSource;

    private bool _shouldReact = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultContract{TData, TState, TError}"/> class.
    /// </summary>
    /// <param name="contractor">The contractor.</param>
    /// <param name="defaultState">The default state.</param>
    /// <param name="fillAtState">The state to fill data at.</param>
    /// <param name="fillData">The function to fill the data.</param>
    /// <param name="actions">The actions to execute at each state.</param>
    /// <param name="timeouts">The timeouts.</param>
    public DefaultContract
    (
        Contractor contractor,
        TState defaultState,
        TState fillAtState,
        FillDataAsync fillData,
        IDictionary<TState, StateActionAsync> actions,
        IDictionary<TState, (TimeSpan Timeout, TState? NextState, TError? Error)> timeouts
    )
    {
        _semaphore = new SemaphoreSlim(1, 1);
        _timeouts = timeouts;
        _defaultState = defaultState;
        _contractor = contractor;
        CurrentState = defaultState;

        _actions = actions;
        _fillData = fillData;
        _fillAtState = fillAtState;
    }

    /// <inheritdoc />
    public TState CurrentState { get; private set; }

    /// <inheritdoc />
    public TData? Data { get; private set; }

    /// <inheritdoc />
    public bool IsRegistered { get; private set; }

    /// <inheritdoc />
    public void Register()
    {
        if (!IsRegistered)
        {
            _contractor.Register(this);
            IsRegistered = true;
        }
    }

    /// <inheritdoc />
    public void Unregister()
    {
        if (IsRegistered)
        {
            _contractor.Unregister(this);
            IsRegistered = false;
        }
    }

    /// <inheritdoc />
    public async Task<Result<ContractUpdateResponse>> Update<TAny>(TAny data, CancellationToken ct = default)
        where TAny : notnull
    {
        if (!_shouldReact || !IsRegistered)
        {
            return ContractUpdateResponse.NotInterested;
        }

        var currentState = CurrentState;
        if (currentState.CompareTo(_defaultState) == 0)
        { // cannot update with default state. OnlyExecute has to be called first and update the state.
            return ContractUpdateResponse.NotInterested;
        }

        if (!_actions.ContainsKey(currentState))
        {
            Unregister();
            return ContractUpdateResponse.NotInterested;
        }

        var result = await _actions[currentState](data, ct);
        if (!result.IsDefined(out var resultData))
        {
            _shouldReact = false;
            _resultError = Result.FromError(result);
            _waitCancellationSource?.Cancel();
            return ContractUpdateResponse.InterestedAndUnregister;
        }

        if (resultData.Error is not null)
        {
            _shouldReact = false;
            _error = resultData.Error;
            try
            {
                _waitCancellationSource?.Cancel();
            }
            catch
            {
                // ignored
            }

            return ContractUpdateResponse.InterestedAndUnregister;
        }

        if (resultData.NextState is null)
        {
            return ContractUpdateResponse.NotInterested;
        }

        await SetCurrentState(resultData.NextState.Value, ct);

        return await SetupNewState(data!, ct);
    }

    /// <inheritdoc />
    public async Task<Result> OnlyExecuteAsync(CancellationToken ct = default)
    {
        if (_actions.ContainsKey(_defaultState))
        {
            var result = await _actions[_defaultState](default, ct);
            if (!result.IsSuccess)
            {
                return Result.FromError(result);
            }

            var (error, state) = result.Entity;

            if (error is not null)
            {
                _error = error;
                _waitCancellationSource?.Cancel();
                return new ContractError<TError>(error.Value);
            }

            if (state is not null)
            {
                await SetCurrentState(state.Value, ct);
                var newStateResult = await SetupNewState<int?>(null, ct);

                if (!newStateResult.IsSuccess)
                {
                    return Result.FromError(newStateResult);
                }
            }
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result<TData>> WaitForAsync
        (TState state, bool unregisterAfter = true, CancellationToken ct = default)
    {
        if (_fillAtState.CompareTo(state) > 0)
        {
            throw new InvalidOperationException
            (
                $"The requested state {state} does not guarantee data filled. The state that fills data is {_defaultState}"
            );
        }

        if (_error is not null)
        {
            return new ContractError<TError>(_error.Value);
        }

        if (_resultError is not null)
        {
            return Result<TData>.FromError(_resultError.Value);
        }

        if (CurrentState.CompareTo(state) >= 0)
        { // already reached.
            return Data;
        }

        _waitingFor = state;
        _unregisterAtWaitingFor = unregisterAfter;
        _waitCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(ct);

        if (CurrentState.CompareTo(_defaultState) == 0)
        {
            var result = await OnlyExecuteAsync(ct);
            if (!result.IsSuccess)
            {
                return Result<TData>.FromError(result);
            }
        }

        Register();

        try
        {
            await Task.Delay(-1, _waitCancellationSource.Token);
        }
        catch
        {
            // ignored
        }
        finally
        {
            if (unregisterAfter)
            {
                Unregister();
            }

            _waitCancellationSource?.Dispose();
        }

        if (ct.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        if (_resultError is not null)
        {
            return Result<TData>.FromError(_resultError.Value);
        }

        if (_error is not null)
        {
            return new ContractError<TError>(_error.Value);
        }

        if (Data is null)
        {
            throw new Exception("Data was null, but shouldn't have. There is an error in DefaultContract.");
        }

        return Data;
    }

    /// <inheritdoc />
    public bool HasReachedState(TState state)
        => _error is not null || _resultError is not null || CurrentState.CompareTo(state) >= 0;

    private async Task<Result<ContractUpdateResponse>> SetupNewState<TAny>(TAny data, CancellationToken ct)
    {
        if (_fillAtState.CompareTo(CurrentState) == 0)
        {
            if (data is not null)
            {
                var filledResult = await _fillData(data, ct);

                if (!filledResult.IsDefined(out var filled))
                {
                    _resultError = Result.FromError(filledResult);
                    _waitCancellationSource?.Cancel();
                    return Result<ContractUpdateResponse>.FromError(filledResult);
                }

                Data = filled;
            }
            else
            {
                throw new InvalidOperationException
                (
                    $"Got to a state {CurrentState} without data, but the state should fill data. That's not possible."
                );
            }
        }
        if (_waitingFor is not null && _waitingFor.Value.CompareTo(CurrentState) == 0)
        {
            // but we are inside of the lock now.
            _waitCancellationSource?.Cancel();

            if (_unregisterAtWaitingFor)
            {
                return ContractUpdateResponse.InterestedAndUnregister;
            }
        }

        SetupTimeout();
        return ContractUpdateResponse.Interested;
    }

    private void SetupTimeout()
    {
        if (_timeouts.ContainsKey(CurrentState))
        {
            var currentState = CurrentState;
            var (timeout, state, error) = _timeouts[CurrentState];

            Task.Run
            (
                async () =>
                {
                    await Task.Delay(timeout);

                    if (CurrentState.CompareTo(currentState) == 0)
                    {
                        if (state is not null)
                        {
                            await SetCurrentState(state.Value);
                            await SetupNewState<int?>(null!, default);
                        }
                        else if (error is not null)
                        {
                            _error = error;
                            _waitCancellationSource?.Cancel();
                        }
                    }
                }
            );
        }
    }

    private async Task SetCurrentState(TState state, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        CurrentState = state;
        _semaphore.Release();
    }
}