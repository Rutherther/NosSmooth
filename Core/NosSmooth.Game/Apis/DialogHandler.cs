//
//  DialogHandler.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Game.Data.Dialogs;
using Remora.Results;

namespace NosSmooth.Game.Apis;

/// <summary>
/// Handles accepting and denying of dialogs.
/// </summary>
public class DialogHandler
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogHandler"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    public DialogHandler(INostaleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Accept the operation the dialog does.
    /// </summary>
    /// <param name="dialog">The opened dialog.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> AcceptAsync(Dialog dialog, CancellationToken ct = default)
        => _client.SendPacketAsync(dialog.AcceptCommand, ct);

    /// <summary>
    /// Try to deny the operation the dialog does.
    /// </summary>
    /// <remarks>
    /// Some dialogs do not allow denying, they are just ignored instead.
    /// </remarks>
    /// <param name="dialog">The opened dialog.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> DenyAsync(Dialog dialog, CancellationToken ct = default)
    {
        if (dialog.DenyCommand is null)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        return _client.SendPacketAsync(dialog.DenyCommand, ct);
    }
}