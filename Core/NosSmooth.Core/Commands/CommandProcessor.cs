//
//  CommandProcessor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Core.Client;
using NosSmooth.Core.Errors;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
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
    /// <param name="client">The NosTale client.</param>
    /// <param name="command">The command to process.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    /// <exception cref="InvalidOperationException">Thrown on critical error.</exception>
    public Task<Result> ProcessCommand(INostaleClient client, ICommand command, CancellationToken ct = default)
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

        return (Task<Result>)boundProcessMethod.Invoke(this, new object[] { client, command, ct })!;
    }

    private async Task<Result> DispatchCommandHandler<TCommand>
    (
        INostaleClient client,
        TCommand command,
        CancellationToken ct = default
    )
        where TCommand : class, ICommand
    {
        using var scope = _provider.CreateScope();
        var beforeResult = await ExecuteBeforeExecutionAsync(scope.ServiceProvider, client, command, ct);
        if (!beforeResult.IsSuccess)
        {
            return beforeResult;
        }

        var commandHandler = scope.ServiceProvider.GetService<ICommandHandler<TCommand>>();
        if (commandHandler is null)
        {
            var result = Result.FromError(new CommandHandlerNotFound(command.GetType()));
            var afterExecutionResult = await ExecuteAfterExecutionAsync
            (
                scope.ServiceProvider,
                client,
                command,
                result,
                ct
            );
            if (!afterExecutionResult.IsSuccess)
            {
                return new AggregateError(result, afterExecutionResult);
            }

            return result;
        }

        var handlerResult = await commandHandler.HandleCommand(command, ct);
        var afterResult = await ExecuteAfterExecutionAsync
        (
            scope.ServiceProvider,
            client,
            command,
            handlerResult,
            ct
        );

        if (!afterResult.IsSuccess && !handlerResult.IsSuccess)
        {
            return new AggregateError(handlerResult, afterResult);
        }

        if (!handlerResult.IsSuccess)
        {
            return handlerResult;
        }

        return afterResult;
    }

    private async Task<Result> ExecuteBeforeExecutionAsync<TCommand>
    (
        IServiceProvider services,
        INostaleClient client,
        TCommand command,
        CancellationToken ct
    )
        where TCommand : ICommand
    {
        var results = await Task.WhenAll
        (
            services.GetServices<IPreCommandExecutionEvent>()
                .Select(x => x.ExecuteBeforeCommandAsync(client, command, ct))
        );

        var errorResults = new List<Result>();
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                errorResults.Add(result);
            }
        }

        return errorResults.Count switch
        {
            1 => errorResults[0],
            0 => Result.FromSuccess(),
            _ => new AggregateError(errorResults.Cast<IResult>().ToArray())
        };
    }

    private async Task<Result> ExecuteAfterExecutionAsync<TCommand>
    (
        IServiceProvider services,
        INostaleClient client,
        TCommand command,
        Result handlerResult,
        CancellationToken ct
    )
        where TCommand : ICommand
    {
        var results = await Task.WhenAll
        (
            services.GetServices<IPostCommandExecutionEvent>()
                .Select(x => x.ExecuteAfterCommandAsync(client, command, handlerResult, ct))
        );

        var errorResults = new List<Result>();
        foreach (var result in results)
        {
            if (!result.IsSuccess)
            {
                errorResults.Add(result);
            }
        }

        return errorResults.Count switch
        {
            1 => errorResults[0],
            0 => Result.FromSuccess(),
            _ => new AggregateError(errorResults.Cast<IResult>().ToArray())
        };
    }
}