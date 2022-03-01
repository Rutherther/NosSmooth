//
//  ErrorCommandEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes.Commands.Events;

/// <inheritdoc />
public class ErrorCommandEvent : IPreCommandExecutionEvent, IPostCommandExecutionEvent
{
    /// <inheritdoc />
    public Task<Result> ExecuteBeforeCommandAsync<TCommand>
        (INostaleClient client, TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        return Task.FromResult<Result>(new FakeError("Error pre command execution"));
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
        return Task.FromResult<Result>(new FakeError("Erro post command execution"));
    }
}