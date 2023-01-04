//
//  CombatManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    private readonly List<CancellationTokenSource> _tokenSource;
    private readonly SemaphoreSlim _semaphore;
    private readonly INostaleClient _client;
    private readonly Game.Game _game;
    private bool _cancelling;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatManager"/> class.
    /// </summary>
    /// <param name="client">The NosTale client.</param>
    /// <param name="game">The game.</param>
    public CombatManager(INostaleClient client, Game.Game game)
    {
        _semaphore = new SemaphoreSlim(1, 1);
        _tokenSource = new List<CancellationTokenSource>();
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

        while (!combatState.ShouldQuit && !ct.IsCancellationRequested)
        {
            var commandResult = await _client.SendCommandAsync
            (
                new AttackCommand
                (
                    currentTarget,
                    async (c) =>
                    {
                        while (!combatState.ShouldQuit && currentTarget == previousTarget)
                        {
                            if (!technique.ShouldContinue(combatState))
                            {
                                combatState.QuitCombat();
                                continue;
                            }

                            var operation = combatState.NextOperation();

                            if (operation is null)
                            { // The operation is null and the step has to be obtained from the technique.
                                var stepResult = technique.HandleCombatStep(combatState);
                                if (!stepResult.IsSuccess)
                                {
                                    return Result.FromError(stepResult);
                                }

                                previousTarget = currentTarget;
                                currentTarget = stepResult.Entity;

                                if (previousTarget != currentTarget)
                                {
                                    continue;
                                }

                                operation = combatState.NextOperation();
                            }

                            if (operation is null)
                            { // The operation could be null just because there is currently not a skill to be used etc.
                                await Task.Delay(5, ct);
                                continue;
                            }

                            Result<CanBeUsedResponse> responseResult;
                            while ((responseResult = operation.CanBeUsed(combatState)).IsSuccess
                                && responseResult.Entity == CanBeUsedResponse.MustWait)
                            { // TODO: wait for just some amount of time
                                await Task.Delay(5, ct);
                            }

                            if (!responseResult.IsSuccess)
                            {
                                return Result.FromError(responseResult);
                            }

                            if (responseResult.Entity == CanBeUsedResponse.WontBeUsable)
                            {
                                return new UnusableOperationError(operation);
                            }

                            var usageResult = await operation.UseAsync(combatState, ct);
                            if (!usageResult.IsSuccess)
                            {
                                var errorHandleResult = technique.HandleError(combatState, operation, usageResult);
                                if (!errorHandleResult.IsSuccess)
                                {
                                    return errorHandleResult;
                                }
                            }
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

    /// <summary>
    /// Register the given cancellation token source to be cancelled on skill use/cancel.
    /// </summary>
    /// <param name="tokenSource">The token source to register.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A task.</returns>
    public async Task RegisterSkillCancellationTokenAsync(CancellationTokenSource tokenSource, CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            _tokenSource.Add(tokenSource);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Unregister the given cancellation token registered using RegisterSkillCancellationToken.
    /// </summary>
    /// <param name="tokenSource">The token source to unregister.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A task.</returns>
    public async Task UnregisterSkillCancellationTokenAsync(CancellationTokenSource tokenSource, CancellationToken ct)
    {
        if (_cancelling)
        {
            return;
        }

        await _semaphore.WaitAsync(ct);
        try
        {
            _tokenSource.Remove(tokenSource);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Cancel all of the skill tokens.
    /// </summary>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A task.</returns>
    internal async Task CancelSkillTokensAsync(CancellationToken ct)
    {
        await _semaphore.WaitAsync(ct);
        _cancelling = true;
        try
        {
            foreach (var tokenSource in _tokenSource)
            {
                try
                {
                    tokenSource.Cancel();
                }
                catch
                {
                    // ignored
                }
            }

            _tokenSource.Clear();
        }
        finally
        {
            _cancelling = false;
            _semaphore.Release();
        }
    }
}