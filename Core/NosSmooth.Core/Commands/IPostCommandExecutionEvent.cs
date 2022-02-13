//
//  IPostCommandExecutionEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using Remora.Results;

namespace NosSmooth.Core.Commands;

/// <summary>
/// Event executed after command handler.
/// </summary>
public interface IPostCommandExecutionEvent
{
    /// <summary>
    /// Execute the command post execution event.
    /// </summary>
    /// <param name="client">The NosTale client.</param>
    /// <param name="command">The command.</param>
    /// <param name="handlerResult">The result from the command handler.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <returns>A result that may or may not succeed.</returns>
    public Task<Result> ExecuteAfterCommandAsync<TCommand>
    (
        INostaleClient client,
        TCommand command,
        Result handlerResult,
        CancellationToken ct = default
    )
        where TCommand : ICommand;
}