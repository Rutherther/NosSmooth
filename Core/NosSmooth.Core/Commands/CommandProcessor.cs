//
//  CommandProcessor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Errors;
using Remora.Results;

namespace NosSmooth.Core.Commands;

/// <summary>
/// Calls <see cref="ICommandHandler"/> for the executing command
/// by using <see cref="IServiceProvider"/> dependency injection.
/// </summary>
public class CommandProcessor
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandProcessor"/> class.
    /// </summary>
    /// <param name="provider">The dependency injection provider.</param>
    public CommandProcessor(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Processes the given command, calling its handler or returning error.
    /// </summary>
    /// <param name="command">The command to process.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    /// <exception cref="InvalidOperationException">Thrown on critical error.</exception>
    public Task<Result> ProcessCommand(ICommand command, CancellationToken ct = default)
    {
        var processMethod = GetType().GetMethod
        (
            nameof(DispatchCommandHandler),
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (processMethod is null)
        {
            throw new InvalidOperationException("Could not find process command generic method in command processor.");
        }

        var boundProcessMethod = processMethod.MakeGenericMethod(command.GetType());

        return (Task<Result>)boundProcessMethod.Invoke(this, new object[] { command, ct })!;
    }

    private Task<Result> DispatchCommandHandler<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : class, ICommand
    {
        using var scope = _provider.CreateScope();
        var commandHandler = scope.ServiceProvider.GetService<ICommandHandler<TCommand>>();
        if (commandHandler is null)
        {
            return Task.FromResult(Result.FromError(new CommandHandlerNotFound(command.GetType())));
        }

        return commandHandler.HandleCommand(command, ct);
    }
}