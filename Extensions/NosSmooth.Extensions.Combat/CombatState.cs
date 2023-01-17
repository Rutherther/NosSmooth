//
//  CombatState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using NosSmooth.Core.Client;
using NosSmooth.Extensions.Combat.Operations;

namespace NosSmooth.Extensions.Combat;

/// <inheritdoc />
internal class CombatState : ICombatState
{
    private readonly Dictionary<OperationQueueType, LinkedList<ICombatOperation>> _operations;
    private readonly Dictionary<OperationQueueType, ICombatOperation> _currentOperations;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombatState"/> class.
    /// </summary>
    /// <param name="client">The NosTale client.</param>
    /// <param name="game">The game.</param>
    /// <param name="combatManager">The combat manager.</param>
    public CombatState(INostaleClient client, Game.Game game, CombatManager combatManager)
    {
        Client = client;
        Game = game;
        CombatManager = combatManager;
        _operations = new Dictionary<OperationQueueType, LinkedList<ICombatOperation>>();
        _currentOperations = new Dictionary<OperationQueueType, ICombatOperation>();
    }

    /// <summary>
    /// Gets whether the combat state should be quit.
    /// </summary>
    public bool ShouldQuit { get; private set; }

    /// <inheritdoc/>
    public CombatManager CombatManager { get; }

    /// <inheritdoc/>
    public Game.Game Game { get; }

    /// <inheritdoc/>
    public INostaleClient Client { get; }

    /// <inheritdoc/>
    public bool IsWaitingOnOperation => _currentOperations.Any(x => !x.Value.IsExecuting());

    /// <summary>
    /// Move to next operation, if available.
    /// </summary>
    /// <param name="queueType">The queue type to move to next operation in.</param>
    /// <returns>Next operation, if any.</returns>
    public ICombatOperation? NextOperation(OperationQueueType queueType)
    {
        if (_operations.ContainsKey(queueType))
        {
            var nextOperation = _operations[queueType].FirstOrDefault();

            if (nextOperation is not null)
            {
                _currentOperations[queueType] = nextOperation;
                return nextOperation;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<ICombatOperation> GetWaitingForOperations()
    {
        return _currentOperations.Values.Where(x => !x.IsExecuting()).ToList();
    }

    /// <inheritdoc/>
    public ICombatOperation? GetCurrentOperation(OperationQueueType queueType)
        => _currentOperations.GetValueOrDefault(queueType);

    /// <inheritdoc/>
    public bool IsExecutingOperation(OperationQueueType queueType, [NotNullWhen(true)] out ICombatOperation? operation)
    {
        operation = GetCurrentOperation(queueType);
        return operation is not null && operation.IsExecuting();
    }

    /// <inheritdoc/>
    public void QuitCombat()
    {
        ShouldQuit = true;
    }

    /// <inheritdoc/>
    public void SetCurrentOperation
        (ICombatOperation operation, bool emptyQueue = false, bool prependCurrentOperationToQueue = false)
    {
        var type = operation.QueueType;

        if (!_operations.ContainsKey(type))
        {
            _operations[type] = new LinkedList<ICombatOperation>();
        }

        if (emptyQueue)
        {
            _operations[type].Clear();
        }

        if (prependCurrentOperationToQueue)
        {
            _operations[type].AddFirst(operation);
            return;
        }

        if (_currentOperations.ContainsKey(type))
        {
            _currentOperations[type].Dispose();
        }

        _currentOperations[type] = operation;
    }

    /// <inheritdoc/>
    public void EnqueueOperation(ICombatOperation operation)
    {
        var type = operation.QueueType;

        if (!_operations.ContainsKey(type))
        {
            _operations[type] = new LinkedList<ICombatOperation>();
        }

        _operations[type].AddLast(operation);
    }

    /// <inheritdoc/>
    public void RemoveOperations(Func<ICombatOperation, bool> filter)
    {
        throw new NotImplementedException();
    }
}