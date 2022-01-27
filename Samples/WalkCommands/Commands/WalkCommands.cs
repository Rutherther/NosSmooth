//
//  WalkCommands.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.ChatCommands;
using NosSmooth.Core.Client;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Extensions;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Server.Chat;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;

namespace WalkCommands.Commands;

/// <summary>
/// Represents command group for walking.
/// </summary>
public class WalkCommands : CommandGroup
{
    private readonly INostaleClient _client;
    private readonly FeedbackService _feedbackService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkCommands"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    /// <param name="feedbackService">The feedback service.</param>
    public WalkCommands(INostaleClient client, FeedbackService feedbackService)
    {
        _client = client;
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Attempts to walk the character to the specified lcoation.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="isCancellable">Whether the user can cancel the operation.</param>
    /// <param name="petSelectors">The pet selectors indices.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    [Command("walk")]
    public async Task<Result> HandleWalkToAsync
    (
        ushort x,
        ushort y,
        bool isCancellable = true,
        [Option('p', "pet")] params int[] petSelectors
    )
    {
        var receiveResult = await _client.ReceivePacketAsync
        (
            new SayPacket(EntityType.Map, 1, SayColor.Red, $"Going to walk to {x} {y}."),
            CancellationToken
        );

        if (!receiveResult.IsSuccess)
        {
            return receiveResult;
        }

        var command = new WalkCommand(x, y, petSelectors, AllowUserCancel: isCancellable);
        var walkResult = await _client.SendCommandAsync(command, CancellationToken);
        if (!walkResult.IsSuccess)
        {
            await _feedbackService.SendErrorMessageAsync($"Could not finish walking. {walkResult.ToFullString()}", CancellationToken);
            await _client.ReceivePacketAsync
            (
                new SayPacket(EntityType.Map, 1, SayColor.Red, "Could not finish walking."),
                CancellationToken
            );
            return walkResult;
        }

        return await _client.ReceivePacketAsync
        (
            new SayPacket(EntityType.Map, 1, SayColor.Red, "Walk has finished successfully."),
            CancellationToken
        );
    }
}
