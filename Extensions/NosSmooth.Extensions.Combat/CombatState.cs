//
//  CombatState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml;
using NosSmooth.Core.Client;
using NosSmooth.Extensions.Combat.Operations;
using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Extensions.Combat;

/// <inheritdoc />
internal class CombatState : ICombatState
{
    private readonly LinkedList<ICombatOperation> _operations;
    private ICombatOperation? _currentOperation;

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
        _operations = new LinkedList<ICombatOperation>();
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
    public void QuitCombat()
    {
        ShouldQuit = true;
    }

    /// <summary>
    /// Make a step in the queue.
    /// </summary>
    /// <returns>The current operation, if any.</returns>
    public ICombatOperation? NextOperation()
    {
        var operation = _currentOperation = _operations.First?.Value;
        if (operation is not null)
        {
            _operations.RemoveFirst();
        }

        return operation;
    }

    /// <inheritdoc/>
    public void SetCurrentOperation
        (ICombatOperation operation, bool emptyQueue = false, bool prependCurrentOperationToQueue = false)
    {
        var current = _currentOperation;
        _currentOperation = operation;

        if (emptyQueue)
        {
            _operations.Clear();
        }

        if (prependCurrentOperationToQueue && current is not null)
        {
            _operations.AddFirst(current);
        }
    }

    /// <inheritdoc/>
    public void EnqueueOperation(ICombatOperation operation)
    {
        _operations.AddLast(operation);
    }

    /// <inheritdoc/>
    public void RemoveOperations(Func<ICombatOperation, bool> filter)
    {
        var node = _operations.First;
        while (node != null)
        {
            var next = node.Next;
            if (filter(node.Value))
            {
                _operations.Remove(node);
            }
            node = next;
        }
    }
}