//
//  WalkCommands.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Enumerations;
using NosCore.Packets.ServerPackets.Chats;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using Remora.Results;

namespace WalkCommands.Commands;

/// <summary>
/// Represents command group for walking.
/// </summary>
public class WalkCommands
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkCommands"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    public WalkCommands(INostaleClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Attempts to walk the character to the specified lcoation.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="isCancellable">Whether the user can cancel the operation.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> HandleWalkToAsync
    (
        int x,
        int y,
        bool isCancellable = true,
        CancellationToken ct = default
    )
    {
        var receiveResult = await _client.ReceivePacketAsync
        (
            new SayPacket
            {
                Type = SayColorType.Red, Message = $"Going to walk to {x} {y}"
            },
            ct
        );

        if (!receiveResult.IsSuccess)
        {
            return receiveResult;
        }

        var command = new WalkCommand(x, y, isCancellable);
        var walkResult = await _client.SendCommandAsync(command, ct);
        if (!walkResult.IsSuccess)
        {
            return walkResult;
        }

        return await _client.ReceivePacketAsync
        (
            new SayPacket
            {
                Type = SayColorType.Red, Message = "Walk has finished successfully."
            },
            ct
        );
    }
}