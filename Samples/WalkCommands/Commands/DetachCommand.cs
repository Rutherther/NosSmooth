//
//  DetachCommand.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Packets.Server.Chat;
using Remora.Results;

namespace WalkCommands.Commands;

/// <summary>
/// Group for detaching command that detaches the dll.
/// </summary>
public class DetachCommand
{
    private readonly CancellationTokenSource _dllStop;
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DetachCommand"/> class.
    /// </summary>
    /// <param name="dllStop">The cancellation token source to stop the client.</param>
    /// <param name="client">The nostale client.</param>
    public DetachCommand(CancellationTokenSource dllStop, INostaleClient client)
    {
        _dllStop = dllStop;
        _client = client;
    }

    /// <summary>
    /// Detach the dll.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> HandleDetach()
    {
        var receiveResult = await _client.ReceivePacketAsync
            (new SayPacket(EntityType.Map, 1, SayColor.Green, "Going to detach!"));

        if (!receiveResult.IsSuccess)
        {
            return receiveResult;
        }

        _dllStop.Cancel();
        return Result.FromSuccess();
    }
}
