//
//  WalkCommandHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
public class WalkCommandHandler : ICommandHandler<WalkCommand>
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
        for (var i = 0; i < command.PetSelectors.Length; i++)
        {
            short xOffset = (short)(-1 + (i % 3));
            short yOffset = (short)(-1 + ((i / 3) % 5));
            if (xOffset == 0 && yOffset == 0)
            {
                yOffset += 2;
            }

            int x = command.TargetX;
            int y = command.TargetY;

            if (x + xOffset > 0)
            {
                x += xOffset;
            }
            if (y + yOffset > 0)
            {
                y += yOffset;
            }

            tasks.Add
            (
                _nostaleClient.SendCommandAsync
                (
                    new PetWalkCommand
                    (
                        command.PetSelectors[i],
                        (short)x,
                        (short)y,
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