//
//  TakeControlCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// Handles <see cref="TakeControlCommand"/>.
/// </summary>
internal class TakeControlCommandHandler : ICommandHandler<TakeControlCommand>
{
    private readonly ControlCommands _commands;

    /// <summary>
    /// Initializes a new instance of the <see cref="TakeControlCommandHandler"/> class.
    /// </summary>
    /// <param name="commands">The control commands.</param>
    public TakeControlCommandHandler(ControlCommands commands)
    {
        _commands = commands;
    }

    /// <inheritdoc />
    public async Task<Result> HandleCommand(TakeControlCommand command, CancellationToken ct = default)
    {
        using var source = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var registrationResult = await _commands.RegisterAsync(command, source, ct);
        if (!registrationResult.IsSuccess)
        {
            return registrationResult;
        }

        var token = source.Token;
        try
        {
            var handlerResult = await command.HandleCallback(token);
            await _commands.FinishAsync(command.Group);

            return handlerResult;
        }
        catch (Exception e)
        {
            return e;
        }
        finally
        {
            if (!source.IsCancellationRequested)
            {
                try
                {
                    source.Cancel();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}