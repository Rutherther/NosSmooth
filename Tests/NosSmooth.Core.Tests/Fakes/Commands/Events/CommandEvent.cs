//
//  CommandEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes.Commands.Events;

/// <inheritdoc />
public class CommandEvent<TInCommand> : IPreCommandExecutionEvent, IPostCommandExecutionEvent
    where TInCommand : ICommand
{
    private readonly Func<TInCommand, Result> _preExecutionHandler;
    private readonly Func<TInCommand, Result, Result> _postExecutionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandEvent{TCommand}"/> class.
    /// </summary>
    /// <param name="preExecutionHandler">The pre execution handler.</param>
    /// <param name="postExecutionHandler">The post execution handler.</param>
    public CommandEvent(Func<TInCommand, Result> preExecutionHandler, Func<TInCommand, Result, Result> postExecutionHandler)
    {
        _preExecutionHandler = preExecutionHandler;
        _postExecutionHandler = postExecutionHandler;
    }

    /// <inheritdoc />
    public Task<Result> ExecuteBeforeCommandAsync<TCommand>
        (INostaleClient client, TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        return Task.FromResult(_preExecutionHandler((TInCommand)(object)command));
    }

    /// <inheritdoc />
    public Task<Result> ExecuteAfterCommandAsync<TCommand>
    (
        INostaleClient client,
        TCommand command,
        Result handlerResult,
        CancellationToken ct = default
    )
        where TCommand : ICommand
    {
        return Task.FromResult(_postExecutionHandler((TInCommand)(object)command, handlerResult));
    }
}