//
//  ControlCommands.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using NosSmooth.Core.Errors;
using Remora.Results;

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// The state of <see cref="TakeControlCommand"/>.
/// </summary>
public class ControlCommands
{
    /// <summary>
    /// The group representing every group.
    /// </summary>
    /// <remarks>
    /// This will cancel all ongoing control operations
    /// upon registration.
    ///
    /// This will also be cancelled if any operation tries
    /// to take control.
    /// </remarks>
    public const string AllGroup = "__all";

    private ConcurrentDictionary<string, CommandData> _data;
    private ConcurrentDictionary<string, SemaphoreSlim> _addSemaphores;
    private ConcurrentDictionary<string, SemaphoreSlim> _removeSemaphores;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlCommands"/> class.
    /// </summary>
    public ControlCommands()
    {
        _data = new ConcurrentDictionary<string, CommandData>();
        _addSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
        _removeSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
    }

    /// <summary>
    /// Gets whether user actions are currently allowed.
    /// </summary>
    public bool AllowUserActions { get; private set; } = true;

    /// <summary>
    /// Register the given command.
    /// </summary>
    /// <remarks>
    /// The command will grant control if the result is successful.
    /// After execution the command should call CancelAsync.
    /// </remarks>
    /// <param name="command">The command data.</param>
    /// <param name="cancellationTokenSource">The cancellation token source that will be cancelled.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>The cancellation token that will symbolize the operation should be cancelled.</returns>
    public async Task<Result> RegisterAsync
        (TakeControlCommand command, CancellationTokenSource cancellationTokenSource, CancellationToken ct = default)
    {
        var semaphore = _addSemaphores.GetOrAdd(command.Group, _ => new SemaphoreSlim(1, 1));
        var removeSempahore = _removeSemaphores.GetOrAdd(command.Group, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(ct);

        var matchingCommands = FindMatchingCommands(command.Group);
        var cancelOperations = new List<Task<Result>>();
        foreach (var matchingCommand in matchingCommands)
        {
            cancelOperations.Add
                (CancelCommandAsync(matchingCommand, command.WaitForCancellation, ControlCommandsFilter.None, ct));
        }

        if (command.WaitForCancellation && cancelOperations.Count > 0)
        {
            semaphore.Release();
        }

        var results = await Task.WhenAll(cancelOperations);
        var errorResults = results.Where(x => !x.IsSuccess).ToArray();

        if (errorResults.Length > 0)
        {
            return errorResults.Length switch
            {
                1 => errorResults[0],
                _ => new AggregateError(errorResults.Cast<IResult>().ToList())
            };
        }

        if (command.WaitForCancellation && cancelOperations.Count > 0)
        {
            // There could be a new take of control already.
            return await RegisterAsync(command, cancellationTokenSource, ct);
        }

        await removeSempahore.WaitAsync(ct); // Should be right away
        _data.TryAdd(command.Group, new CommandData(command, cancellationTokenSource));
        cancellationTokenSource.Token.Register
        (
            () =>
            {
                _data.TryRemove(command.Group, out _);
                removeSempahore.Release();
            }
        );
        semaphore.Release();
        if (!command.AllowUserCancel)
        {
            AllowUserActions = false;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Finish command from the given group gracefully.
    /// </summary>
    /// <param name="group">The group to finish.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> FinishAsync(string group)
    {
        var commandsToFinish = FindMatchingCommands(group);
        return FinishCommandsAsync(commandsToFinish);
    }

    /// <summary>
    /// Cancel the given group command.
    /// </summary>
    /// <param name="group">The name of the group to cancel the command.</param>
    /// <param name="waitForCancellation">Whether to wait if the ongoing operation is not cancellable.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> CancelAsync(string group, bool waitForCancellation = true, CancellationToken ct = default)
    {
        var commandsToCancel = FindMatchingCommands(group);
        return CancelCommandsAsync(commandsToCancel, waitForCancellation, ct: ct);
    }

    /// <summary>
    /// Cancel the given commands.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="waitForCancellation">Whether to wait for cancellation of non cancellable commands.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> CancelAsync
        (ControlCommandsFilter filter, bool waitForCancellation = true, CancellationToken ct = default)
    {
        bool cancelUser = filter.HasFlag(ControlCommandsFilter.UserCancellable);
        bool cancelMapChanged = filter.HasFlag(ControlCommandsFilter.MapChangeCancellable);
        bool cancelAll = !cancelUser && !cancelMapChanged;

        var commandsToCancel = _data.Values.Where
        (
            x => cancelAll || (cancelUser && x.Command.AllowUserCancel)
                || (cancelMapChanged && x.Command.CancelOnMapChange)
        );

        return await CancelCommandsAsync(commandsToCancel, waitForCancellation, filter, ct);
    }

    private async Task<Result> FinishCommandsAsync(IEnumerable<CommandData> commandsToFinish)
    {
        var tasks = commandsToFinish.Select(x => FinishCommandAsync(x, null));
        var results = await Task.WhenAll(tasks);
        var errorResults = results.Where(x => !x.IsSuccess).ToArray();

        return errorResults.Length switch
        {
            0 => Result.FromSuccess(),
            1 => errorResults[0],
            _ => new AggregateError(errorResults.Cast<IResult>().ToList())
        };
    }

    private async Task<Result> FinishCommandAsync(CommandData data, ControlCancelReason? cancelReason)
    {
        Result cancelledResult = Result.FromSuccess();
        if (cancelReason is not null)
        {
            try
            {
                 cancelledResult = await data.Command.CancelledCallback((ControlCancelReason)cancelReason);
            }
            catch (Exception e)
            {
                cancelledResult = e;
            }
        }

        try
        {
            if (!data.CancellationTokenSource.IsCancellationRequested)
            {
                data.CancellationTokenSource.Cancel();
            }
        }
        catch
        {
            // Don't handle
        }

        if (!AllowUserActions && !data.Command.AllowUserCancel)
        {
            AllowUserActions = _data.Values.All(x => x.Command.AllowUserCancel);
        }

        return cancelledResult;
    }

    private async Task<Result> CancelCommandsAsync
    (
        IEnumerable<CommandData> data,
        bool waitForCancellation = true,
        ControlCommandsFilter filter = ControlCommandsFilter.None,
        CancellationToken ct = default
    )
    {
        var commands = data.ToArray();
        if (commands.Length == 0)
        {
            return Result.FromSuccess();
        }

        if (commands.Length == 1)
        {
            return await CancelCommandAsync(commands[0], waitForCancellation, filter, ct);
        }

        var tasks = new List<Task<Result>>();
        foreach (var command in commands)
        {
            tasks.Add(CancelCommandAsync(command, waitForCancellation, filter, ct));
        }

        var results = await Task.WhenAll(tasks);
        var errorResults = results.Where(x => !x.IsSuccess).ToArray();
        return errorResults.Length switch
        {
            1 => errorResults[0],
            _ => new AggregateError(errorResults.Cast<IResult>().ToArray())
        };
    }

    private async Task<Result> CancelCommandAsync
    (
        CommandData data,
        bool waitForCancellation = true,
        ControlCommandsFilter filter = ControlCommandsFilter.None,
        CancellationToken ct = default
    )
    {
        if (!data.Command.CanBeCancelledByAnother && !filter.HasFlag(ControlCommandsFilter.UserCancellable))
        {
            if (!waitForCancellation)
            {
                return Result.FromError(new CouldNotGainControlError(data.Command.Group, "would wait"));
            }

            // Wait for the successful finish.
            var found = _removeSemaphores.TryGetValue(data.Command.Group, out var semaphore);
            if (!found || semaphore is null)
            {
                return Result.FromError
                    (new CouldNotGainControlError(data.Command.Group, "did not find remove semaphore. Bug?"));
            }

            await semaphore.WaitAsync(ct);
            semaphore.Release();
        }

        var cancelReason = filter switch
        {
            ControlCommandsFilter.UserCancellable => ControlCancelReason.UserAction,
            ControlCommandsFilter.MapChangeCancellable => ControlCancelReason.MapChanged,
            _ => ControlCancelReason.AnotherTask
        };

        return await FinishCommandAsync
        (
            data,
            cancelReason
        );
    }

    private IEnumerable<CommandData> FindMatchingCommands(string group)
    {
        if (group == AllGroup)
        {
            return _data.Values.ToArray();
        }

        if (!_data.ContainsKey(group))
        {
            return Array.Empty<CommandData>();
        }

        return new[] { _data[group] };
    }

    private struct CommandData
    {
        public CommandData(TakeControlCommand command, CancellationTokenSource cancellationTokenSource)
        {
            Command = command;
            CancellationTokenSource = cancellationTokenSource;
        }

        public TakeControlCommand Command { get; }

        public CancellationTokenSource CancellationTokenSource { get; }
    }
}