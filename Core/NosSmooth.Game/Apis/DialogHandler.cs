//
//  DialogHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using NosSmooth.Core.Client;
using NosSmooth.Game.Data.Dialogs;
using NosSmooth.Game.Errors;
using Remora.Results;

namespace NosSmooth.Game.Apis;

/// <summary>
/// Handles accepting and denying of dialogs.
/// </summary>
public class DialogHandler : IDisposable
{
    private static readonly TimeSpan ForgetAfter = TimeSpan.FromMinutes(1);

    private readonly INostaleClient _client;
    private readonly ConcurrentQueue<DialogAnswer> _answers;
    private readonly SemaphoreSlim _taskSemaphore;
    private readonly SemaphoreSlim _enqueueSemaphore;
    private CancellationTokenSource? _dialogCleanupCancel;
    private Task? _dialogCleanupTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogHandler"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    public DialogHandler(INostaleClient client)
    {
        _enqueueSemaphore = new SemaphoreSlim(1, 1);
        _taskSemaphore = new SemaphoreSlim(1, 1);
        _answers = new ConcurrentQueue<DialogAnswer>();
        _client = client;
    }

    /// <summary>
    /// Accept the operation the dialog does.
    /// </summary>
    /// <param name="dialog">The opened dialog.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> AcceptAsync(Dialog dialog, CancellationToken ct = default)
    {
        var handleResult = await HandleDialogAnswer(dialog, true, ct);

        if (handleResult.SendPacket)
        {
            return await _client.SendPacketAsync(dialog.AcceptCommand, ct);
        }

        return handleResult.AnsweredSame
            ? Result.FromSuccess()
            : new DialogConflictError(dialog, false, true);
    }

    /// <summary>
    /// Try to deny the operation the dialog does.
    /// </summary>
    /// <remarks>
    /// Some dialogs do not allow denying, they are just ignored instead.
    /// </remarks>
    /// <param name="dialog">The opened dialog.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> DenyAsync(Dialog dialog, CancellationToken ct = default)
    {
        var handleResult = await HandleDialogAnswer(dialog, false, ct);

        if (handleResult.SendPacket && dialog.DenyCommand is not null)
        {
            return await _client.SendPacketAsync(dialog.DenyCommand, ct);
        }

        return handleResult.AnsweredSame
            ? Result.FromSuccess()
            : new DialogConflictError(dialog, true, false);
    }

    private async Task<(bool AnsweredSame, bool SendPacket)> HandleDialogAnswer
        (Dialog dialog, bool newAnswer, CancellationToken ct)
    {
        // there could be two concurrent answers, both different,
        // because of that we have to lock here as well...
        await _enqueueSemaphore.WaitAsync(ct);
        try
        {
            bool? answerSame = null;
            foreach (var answer in _answers)
            {
                if (answer.Dialog == dialog)
                {
                    answerSame = answer.Accept == newAnswer;
                    break;
                }
            }

            if (answerSame is null)
            {
                _answers.Enqueue(new DialogAnswer(dialog, newAnswer, DateTimeOffset.Now));
                await StartQueueHandler();
            }

            return answerSame switch
            {
                true => (true, false),
                false => (false, false),
                null => (true, true)
            };
        }
        finally
        {
            _enqueueSemaphore.Release();
        }
    }

    private async Task StartQueueHandler()
    {
        await _taskSemaphore.WaitAsync();
        if (_dialogCleanupTask is null)
        {
            _dialogCleanupCancel?.Dispose();
            _dialogCleanupCancel = new CancellationTokenSource();
            _dialogCleanupTask = Task.Run(() => DeletionTask(_dialogCleanupCancel.Token));
        }
        _taskSemaphore.Release();
    }

    private async Task DeletionTask(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            // taskSemaphore is for ensuring a task will always be started in case
            // it is needed.
            await _taskSemaphore.WaitAsync(ct);
            if (!_answers.TryPeek(out var answer))
            {
                _dialogCleanupTask = null;
                _taskSemaphore.Release();
                return;
            }
            _taskSemaphore.Release();

            DateTimeOffset deleteAt = answer.AnsweredAt.Add(ForgetAfter);

            if (DateTimeOffset.Now >= deleteAt)
            {
                _answers.TryDequeue(out _);

                // nothing else dequeues, the time is not changing.
            }

            // tasks are in chronological order => we can wait for the first one without any issue.
            await Task.Delay(deleteAt - DateTimeOffset.Now + TimeSpan.FromMilliseconds(10), ct);
        }
    }

    private record DialogAnswer(Dialog Dialog, bool Accept, DateTimeOffset AnsweredAt);

    /// <inheritdoc />
    public void Dispose()
    {
        _taskSemaphore.Dispose();
        _dialogCleanupCancel?.Cancel();
        _dialogCleanupCancel = null;
        _dialogCleanupTask = null;
    }
}