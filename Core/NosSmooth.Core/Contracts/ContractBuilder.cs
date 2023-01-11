//
//  ContractBuilder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

    private TState? _fillAtState;
    private DefaultContract<TData, TState, TError>.FillDataAsync? _fillData;
    private Dictionary<TState, DefaultContract<TData, TState, TError>.StateActionAsync> _actions;

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
    }

    public ContractBuilder SetMoveFilter<TAny>(TState state, Func<TAny, bool> filter, TState nextState)
    {

    }
    
    public ContractBuilder SetMoveFilter<TAny>(TState state, TState nextState)
    {

    }

    public ContractBuilder SetFillData<TAny>(TState state, Func<TAny, Result<TData>> fillData)
    {
        _fillAtState = state;
        _fillData = fillData;
    }
    
    public ContractBuilder SetError<TAny>(TState state, Func<TAny, TError?> error, TState nextState)
    {

    }

    public ContractBuilder SetMoveAction<TAny>(TState state, Func<TAny, Task<Result<bool>>> actionFilter, TState nextState)
    {

    }

    public ContractBuilder SetPacketError<TAny>(TState state, Func<TAny, TError?> error, TState nextState)
    {

    }

    public IContract Build()
    {
        new DefaultContract<TData, TState, TError>()
    }
}