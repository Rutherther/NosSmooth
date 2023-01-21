//
//  CombatManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands.Attack;
using NosSmooth.Core.Stateful;
using NosSmooth.Extensions.Combat.Errors;
using NosSmooth.Extensions.Combat.Operations;
using NosSmooth.Extensions.Combat.Techniques;
using Remora.Results;

namespace NosSmooth.Extensions.Combat;

/// <summary>
/// The combat manager that uses techniques to attack enemies.
/// </summary>
public class CombatManager : IStatefulEntity
{
    private readonly INostaleClient _client;
    private readonly Game.Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatManager"/> class.
    /// </summary>
    /// <param name="client">The NosTale client.</param>
    /// <param name="game">The game.</param>
    public CombatManager(INostaleClient client, Game.Game game)
    {
        _client = client;
        _game = game;
    }

    /// <summary>
    /// Enter into a combat state using the given technique.
    /// </summary>
    /// <param name="technique">The technique to use.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not succeed.</returns>
    public async Task<Result> EnterCombatAsync(ICombatTechnique technique, CancellationToken ct = default)
    {
        var combatState = new CombatState(_client, _game, this);
        long? currentTarget = null;
        long? previousTarget = null;

        while (!(combatState.ShouldQuit && combatState.CanQuit) && !ct.IsCancellationRequested)
        {
            var commandResult = await _client.SendCommandAsync
            (
                new AttackCommand
                (
                    currentTarget,
                    async (ct) =>
                    {
                        while (!(combatState.ShouldQuit && combatState.CanQuit) && currentTarget == previousTarget)
                        {
                            var iterationResult = await HandleAttackIterationAsync(combatState, technique, ct);

                            if (!iterationResult.IsSuccess)
                            {
                                var errorResult = technique.HandleError(combatState, Result.FromError(iterationResult));

                                if (!errorResult.IsSuccess)
                                { // end the attack.
                                    return errorResult;
                                }
                            }

                            var result = iterationResult.Entity;
                            if (!result.TargetChanged)
                            {
                                continue;
                            }

                            previousTarget = currentTarget;
                            currentTarget = result.TargetId;
                        }

                        return Result.FromSuccess();
                    }
                ),
                ct
            );

            if (!commandResult.IsSuccess)
            {
                return commandResult;
            }

            previousTarget = currentTarget;
        }
        return Result.FromSuccess();
    }

    private async Task<Result<(bool TargetChanged, long? TargetId)>> HandleAttackIterationAsync
        (CombatState combatState, ICombatTechnique technique, CancellationToken ct)
    {
        if (!technique.ShouldContinue(combatState))
        {
            combatState.QuitCombat();
        }

        // the operations need time for execution and/or
        // wait.
        await Task.Delay(50, ct);

        var tasks = technique.HandlingQueueTypes
            .Select(x => HandleTypeIterationAsync(x, combatState, technique, ct))
            .ToArray();

        var results = await Task.WhenAll(tasks);
        var errors = results.Where(x => !x.IsSuccess).Cast<IResult>().ToArray();

        return errors.Length switch
        {
            0 => results.FirstOrDefault
                (x => x.Entity.TargetChanged, Result<(bool TargetChanged, long?)>.FromSuccess((false, null))),
            1 => (Result<(bool, long?)>)errors[0],
            _ => new AggregateError()
        };
    }

    private async Task<Result<(bool TargetChanged, long? TargetId)>> HandleTypeIterationAsync
    (
        OperationQueueType queueType,
        CombatState combatState,
        ICombatTechnique technique,
        CancellationToken ct
    )
    {
        var currentOperation = combatState.GetCurrentOperation(queueType);
        if (currentOperation?.IsFinished() ?? false)
        {
            var operationResult = await currentOperation.WaitForFinishedAsync(combatState, ct);
            currentOperation.Dispose();

            if (!operationResult.IsSuccess)
            {
                return Result<(bool, long?)>.FromError(operationResult);
            }

            currentOperation = null;
        }

        if (currentOperation is null && !combatState.ShouldQuit)
        { // waiting for an operation.
            currentOperation = combatState.NextOperation(queueType);

            if (currentOperation is null)
            { // The operation is null and the step has to be obtained from the technique.
                var stepResult = technique.HandleNextCombatStep(queueType, combatState);
                if (!stepResult.IsSuccess)
                {
                    return Result<(bool, long?)>.FromError(stepResult);
                }

                return Result<(bool, long?)>.FromSuccess((true, stepResult.Entity));
            }
        }

        if (currentOperation is null)
        { // should quit, do nothing.
            return (false, null);
        }

        if (!currentOperation.IsExecuting())
        { // not executing, check can be used, execute if can.
            var canBeUsedResult = currentOperation.CanBeUsed(combatState);
            if (!canBeUsedResult.IsDefined(out var canBeUsed))
            {
                return Result<(bool, long?)>.FromError(canBeUsedResult);
            }

            switch (canBeUsed)
            {
                case CanBeUsedResponse.WontBeUsable:
                    return new UnusableOperationError(currentOperation);
                case CanBeUsedResponse.MustWait:
                    var waitingResult = technique.HandleWaiting(queueType, combatState, currentOperation);

                    if (!waitingResult.IsSuccess)
                    {
                        return Result<(bool, long?)>.FromError(waitingResult);
                    }

                    return Result<(bool, long?)>.FromSuccess((false, null));
                case CanBeUsedResponse.CanBeUsed:
                    var executingResult = await currentOperation.BeginExecution(combatState, ct);

                    if (!executingResult.IsSuccess)
                    {
                        return Result<(bool, long?)>.FromError(executingResult);
                    }
                    break;
            }
        }

        return (false, null);
    }
}