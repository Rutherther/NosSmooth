//
//  IPreCommandExecutionEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Commands;

/// <summary>
/// Event executed prior to command handler.
/// </summary>
public interface IPreCommandExecutionEvent
{
    /// <summary>
    /// Execute the command pre execution event.
    /// </summary>
    /// <remarks>
    /// If an error is returned, the command handler won't be called.
    /// </remarks>
    /// <param name="client">The NosTale client.</param>
    /// <param name="command">The command.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <returns>A result that may or may not succeed.</returns>
    public Task<Result> ExecuteBeforeCommandAsync<TCommand>
    (
        INostaleClient client,
        TCommand command,
        CancellationToken ct = default
    )
        where TCommand : ICommand;
}