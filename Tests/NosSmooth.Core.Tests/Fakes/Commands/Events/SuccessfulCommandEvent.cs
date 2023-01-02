//
//  SuccessfulCommandEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Tests.Fakes.Commands.Events;

/// <inheritdoc />
public class SuccessfulCommandEvent : IPreCommandExecutionEvent, IPostCommandExecutionEvent
{
    /// <inheritdoc />
    public Task<Result> ExecuteBeforeCommandAsync<TCommand>
        (INostaleClient client, TCommand command, CancellationToken ct = default)
        where TCommand : ICommand
    {
        return Task.FromResult(Result.FromSuccess());
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
        return Task.FromResult(Result.FromSuccess());
    }
}