//
//  WalkCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using Remora.Results;

namespace NosSmooth.Core.Commands.Walking;

/// <summary>
/// Handles <see cref="WalkCommand"/>.
/// </summary>
internal class WalkCommandHandler : ICommandHandler<WalkCommand>
{
    private readonly INostaleClient _nostaleClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkCommandHandler"/> class.
    /// </summary>
    /// <param name="nostaleClient">The NosTale client.</param>
    public WalkCommandHandler(INostaleClient nostaleClient)
    {
        _nostaleClient = nostaleClient;
    }

    /// <inheritdoc />
    public async Task<Result> HandleCommand(WalkCommand command, CancellationToken ct = default)
    {
        var tasks = new List<Task<Result>>();
        foreach (var pet in command.Pets ?? Array.Empty<(long, short, short)>())
        {
            int x = pet.TargetX;
            int y = pet.TargetY;

            tasks.Add
            (
                _nostaleClient.SendCommandAsync
                (
                    new MateWalkCommand
                    (
                        pet.MateId,
                        (short)x,
                        (short)y,
                        command.ReturnDistanceTolerance,
                        command.CanBeCancelledByAnother,
                        command.WaitForCancellation,
                        command.AllowUserCancel
                    ),
                    ct
                )
            );
        }

        tasks.Add
        (
            _nostaleClient.SendCommandAsync
            (
                new PlayerWalkCommand
                (
                    command.TargetX,
                    command.TargetY,
                    command.ReturnDistanceTolerance,
                    command.CanBeCancelledByAnother,
                    command.WaitForCancellation,
                    command.AllowUserCancel
                ),
                ct
            )
        );

        var results = await Task.WhenAll(tasks);
        var errorResults = results.Where(x => !x.IsSuccess).ToArray();

        return errorResults.Length switch
        {
            0 => Result.FromSuccess(),
            1 => errorResults[0],
            _ => new AggregateError(errorResults.Cast<IResult>().ToArray())
        };
    }
}