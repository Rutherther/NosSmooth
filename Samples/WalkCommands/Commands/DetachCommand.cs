//
//  DetachCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.ChatCommands;
using NosSmooth.Core.Client;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Server.Chat;
using Remora.Commands.Groups;
using Remora.Results;

namespace WalkCommands.Commands;

/// <summary>
/// Group for detaching command that detaches the dll.
/// </summary>
public class DetachCommand : CommandGroup
{
    private readonly CancellationTokenSource _dllStop;
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DetachCommand"/> class.
    /// </summary>
    /// <param name="dllStop">The cancellation token source to stop the client.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public DetachCommand(CancellationTokenSource dllStop, FeedbackService feedbackService)
    {
        _dllStop = dllStop;
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Detach the dll.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> HandleDetach()
    {
        var receiveResult = await _feedbackService.SendInfoMessageAsync("Going to detach!", CancellationToken);

        if (!receiveResult.IsSuccess)
        {
            return receiveResult;
        }

        _dllStop.Cancel();
        return Result.FromSuccess();
    }
}
