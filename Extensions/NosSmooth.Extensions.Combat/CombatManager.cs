//
//  CombatManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
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
    private readonly Semaphore _semaphore;
    private readonly INostaleClient _client;
    private readonly Game.Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatManager"/> class.
    /// </summary>
    /// <param name="client">The NosTale client.</param>
    /// <param name="game">The game.</param>
    public CombatManager(INostaleClient client, Game.Game game)
    {
        _semaphore = new Semaphore(1, 1);
        _tokenSource = new List<CancellationTokenSource>();
        _client = client;
        _game = game;
    }

    /// <summary>
    /// Enter into a combat state using the given technique.
    /// </summary>
    /// <param name="technique">The technique to use.</param>
    /// <returns>A result that may or may not succeed.</returns>
    public async Task<Result> EnterCombat(ICombatTechnique technique)
    {
        var combatState = new CombatState(_client, _game, this);

        while (!combatState.ShouldQuit)
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
                    return stepResult;
                }

                operation = combatState.NextOperation();
            }

            if (operation is null)
            { // The operation could be null just because there is currently not a skill to be used etc.
                await Task.Delay(5);
                continue;
            }

            Result<CanBeUsedResponse> responseResult;
            while ((responseResult = operation.CanBeUsed(combatState)).IsSuccess
                && responseResult.Entity == CanBeUsedResponse.MustWait)
            { // TODO: wait for just some amount of time
                await Task.Delay(5);
            }

            if (!responseResult.IsSuccess)
            {
                return Result.FromError(responseResult);
            }

            if (responseResult.Entity == CanBeUsedResponse.WontBeUsable)
            {
                return new UnusableOperationError(operation);
            }

            var usageResult = await operation.UseAsync(combatState);
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

    /// <summary>
    /// Register the given cancellation token source to be cancelled on skill use/cancel.
    /// </summary>
    /// <param name="tokenSource">The token source to register.</param>
    public void RegisterSkillCancellationToken(CancellationTokenSource tokenSource)
    {
        _semaphore.WaitOne();
        _tokenSource.Add(tokenSource);
        _semaphore.Release();
    }

    /// <summary>
    /// Unregister the given cancellation token registered using <see cref="RegisterSkillCancellationToken"/>.
    /// </summary>
    /// <param name="tokenSource">The token source to unregister.</param>
    public void UnregisterSkillCancellationToken(CancellationTokenSource tokenSource)
    {
        _semaphore.WaitOne();
        _tokenSource.Remove(tokenSource);
        _semaphore.Release();
    }

    /// <summary>
    /// Cancel all of the skill tokens.
    /// </summary>
    internal void CancelSkillTokens()
    {
        _semaphore.WaitOne();
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
        _semaphore.Release();
    }
}