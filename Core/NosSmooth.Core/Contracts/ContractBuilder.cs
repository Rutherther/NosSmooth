//
//  ContractBuilder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Contracts;

/// <summary>
/// Builds <see cref="IContract"/> with given states
/// and errors.
/// </summary>
/// <typeparam name="TData">The data type.</typeparam>
/// <typeparam name="TState">The states.</typeparam>
/// <typeparam name="TError">The errors that may be returned.</typeparam>
public class ContractBuilder<TData, TState, TError>
    where TState : struct, IComparable
    where TError : struct
    where TData : notnull
{
    private readonly Contractor _contractor;
    private readonly TState _defaultState;

    private readonly Dictionary<TState, DefaultContract<TData, TState, TError>.StateActionAsync> _actions;
    private readonly Dictionary<TState, (TimeSpan, TState)> _timeouts;

    private TState? _fillAtState;
    private DefaultContract<TData, TState, TError>.FillDataAsync? _fillData;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractBuilder{TData, TState, TError}"/> class.
    /// </summary>
    /// <param name="contractor">The contractor.</param>
    /// <param name="defaultState">The default state of the contract.</param>
    public ContractBuilder(Contractor contractor, TState defaultState)
    {
        _contractor = contractor;
        _defaultState = defaultState;
        _actions = new Dictionary<TState, DefaultContract<TData, TState, TError>.StateActionAsync>();
        _timeouts = new Dictionary<TState, (TimeSpan, TState)>();
    }

    /// <summary>
    /// Sets timeout of the given state.
    /// </summary>
    /// <param name="state">The state to set timeout for.</param>
    /// <param name="timeout">The timeout span.</param>
    /// <param name="nextState">The state to go to after timeout.</param>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetTimeout(TState state, TimeSpan timeout, TState nextState)
    {
        _timeouts[state] = (timeout, nextState);
        return this;
    }

    /// <summary>
    /// Set up an action filter that works for the given <paramref name="state"/>
    /// If the filter matches, moves to the given state from <paramref name="nextState"/>.
    /// </summary>
    /// <param name="state">The state to apply filter action to.</param>
    /// <param name="filter">The filter to match.</param>
    /// <param name="nextState">The state to move to.</param>
    /// <typeparam name="TAny">The type of the filter data.</typeparam>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetMoveFilter<TAny>
        (TState state, Func<TAny, bool> filter, TState nextState)
    {
        _actions[state] = (data, ct) =>
        {
            if (data is TAny matched)
            {
                if (filter(matched))
                {
                    return Task.FromResult
                        (Result<(TError?, TState?)>.FromSuccess((null, nextState)));
                }
            }

            return Task.FromResult
                (Result<(TError?, TState?)>.FromSuccess((null, null)));
        };
        return this;
    }

    /// <summary>
    /// Set up an action filter that works for the given <paramref name="state"/>
    /// If the filter matches, moves to the given state from <paramref name="nextState"/>.
    /// </summary>
    /// <param name="state">The state to apply filter action to.</param>
    /// <param name="nextState">The state to move to.</param>
    /// <typeparam name="TAny">The type of the filter data.</typeparam>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetMoveFilter<TAny>(TState state, TState nextState)
        => SetMoveFilter<TAny>(state, d => true, nextState);

    /// <summary>
    /// Sets that the given state will fill the data.
    /// </summary>
    /// <param name="state">The state that will fill the data.</param>
    /// <param name="fillData">The function to fill the data.</param>
    /// <typeparam name="TAny">The type that is expected to fill the data.</typeparam>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetFillData<TAny>(TState state, Func<TAny, Result<TData>> fillData)
    {
        _fillAtState = state;
        _fillData = (data, ct) =>
        {
            if (data is not TAny matched)
            {
                return Task.FromResult(Result<TData>.FromError(new GenericError("Fill data not matched.")));
            }

            return Task.FromResult(fillData(matched));
        };
        return this;
    }

    /// <summary>
    /// Sets that the given state should error on the given type.
    /// </summary>
    /// <param name="state">The state to accept error at.</param>
    /// <param name="errorFunction">The error function.</param>
    /// <typeparam name="TAny">The type to match.</typeparam>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetError<TAny>(TState state, Func<TAny, TError?> errorFunction)
    {
        var last = _actions[state];
        _actions[state] = async (data, ct) =>
        {
            if (data is TAny matched)
            {
                var error = errorFunction(matched);

                if (error is not null)
                {
                    return Result<(TError?, TState?)>.FromSuccess((error, null));
                }
            }

            return await last(data, ct);
        };
        return this;
    }

    /// <summary>
    /// Set up an action that works for the given <paramref name="state"/>
    /// If the given state is reached and data are updated, this function is called.
    /// </summary>
    /// <param name="state">The state to apply filter action to.</param>
    /// <param name="actionFilter">The filter to filter the action.</param>
    /// <param name="nextState">The state to move to.</param>
    /// <typeparam name="TAny">The type of the filter data.</typeparam>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetMoveAction<TAny>
        (TState state, Func<TAny, Task<Result<bool>>> actionFilter, TState nextState)
    {
        _actions[state] = async (data, ct) =>
        {
            if (data is TAny matched)
            {
                var filterResult = await actionFilter(matched);
                if (!filterResult.IsDefined(out var filter))
                {
                    return Result<(TError?, TState?)>.FromError(filterResult);
                }

                if (filter)
                {
                    return Result<(TError?, TState?)>.FromSuccess((null, nextState));
                }
            }

            return Result<(TError?, TState?)>.FromSuccess((null, null));
        };
        return this;
    }

    /// <summary>
    /// Set up an action that works for the given <paramref name="state"/>
    /// If the given state is reached and data are updated, this function is called.
    /// </summary>
    /// <param name="state">The state to apply filter action to.</param>
    /// <param name="actionFilter">The filter to filter the action.</param>
    /// <param name="nextState">The state to move to.</param>
    /// <returns>The updated builder.</returns>
    public ContractBuilder<TData, TState, TError> SetMoveAction
        (TState state, Func<object?, CancellationToken, Task<Result<bool>>> actionFilter, TState nextState)
    {
        _actions[state] = async (data, ct) =>
        {
            var filterResult = await actionFilter(data, ct);
            if (!filterResult.IsDefined(out var filter))
            {
                return Result<(TError?, TState?)>.FromError(filterResult);
            }

            if (filter)
            {
                return Result<(TError?, TState?)>.FromSuccess((null, nextState));
            }

            return Result<(TError?, TState?)>.FromSuccess((null, null));
        };
        return this;
    }

    /// <summary>
    /// Build the associate contract.
    /// </summary>
    /// <returns>The contract.</returns>
    /// <exception cref="InvalidOperationException">Thrown in case FillAtState or FillData is null.</exception>
    public IContract<TData, TState> Build()
    {
        if (_fillAtState is null)
        {
            throw new InvalidOperationException("FillAtState cannot be null.");
        }

        if (_fillData is null)
        {
            throw new InvalidOperationException("FillData cannot be null.");
        }

        return new DefaultContract<TData, TState, TError>
        (
            _contractor,
            _defaultState,
            _fillAtState.Value,
            _fillData,
            _actions,
            _timeouts
        );
    }
}