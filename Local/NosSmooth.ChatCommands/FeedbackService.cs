//
//  FeedbackService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Server.Chat;
using Remora.Results;

namespace NosSmooth.ChatCommands;

/// <summary>
/// Feedback for chat commands.
/// </summary>
public class FeedbackService
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeedbackService"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    public FeedbackService(INostaleClient client)
    {
        _client = client;

    }

    /// <summary>
    /// Send message error.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendErrorMessageAsync(string message, CancellationToken ct = default)
        => SendMessageAsync(message, SayColor.Red, ct);

    /// <summary>
    /// Send message success.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendSuccessMessageAsync(string message, CancellationToken ct = default)
        => SendMessageAsync(message, SayColor.Green, ct);

    /// <summary>
    /// Send message info.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendInfoMessageAsync(string message, CancellationToken ct = default)
        => SendMessageAsync(message, SayColor.Default, ct);

    /// <summary>
    /// Send message with the given color.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="color">The color.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendMessageAsync(string message, SayColor color, CancellationToken ct = default)
        => _client.ReceivePacketAsync(new SayPacket(EntityType.Map, 0, color, message), ct);
}