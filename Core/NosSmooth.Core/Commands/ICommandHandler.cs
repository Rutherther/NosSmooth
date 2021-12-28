//
//  ICommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Commands;

/// <summary>
/// Represents interface for command handlers that process <see cref="ICommand"/>.
/// </summary>
public interface ICommandHandler
{
}

/// <summary>
/// Represents interface of class that handles <see cref="ICommand"/> of type <typeparamref name="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">The command type that this handler can execute.</typeparam>
public interface ICommandHandler<TCommand> : ICommandHandler
    where TCommand : ICommand
{
    /// <summary>
    /// Execute the given command.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> HandleCommand(TCommand command, CancellationToken ct = default);
}