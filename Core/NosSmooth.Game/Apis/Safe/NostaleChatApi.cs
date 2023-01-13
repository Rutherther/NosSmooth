//
//  NostaleChatApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Chat;
using Remora.Results;

namespace NosSmooth.Game.Apis.Safe;

/// <summary>
/// Packet api for sending and receiving messages.
/// </summary>
public class NostaleChatApi
{
    // TODO: check length of the messages
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleChatApi"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    public NostaleChatApi(INostaleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Receive the given system message on the client.
    /// </summary>
    /// <remarks>
    /// Won't send anything to the server, it's just the client who will see the message.
    /// </remarks>
    /// <param name="content">The content of the message.</param>
    /// <param name="color">The color of the message.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> ReceiveSystemMessageAsync(string content, SayColor color = SayColor.Yellow, CancellationToken ct = default)
        => _client.ReceivePacketAsync(new SayPacket(EntityType.Map, 0, color, content), ct);

    /// <summary>
    /// Sends the given message to the public chat.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendMessageAsync(string content, CancellationToken ct = default)
        => _client.SendPacketAsync(new Packets.Client.Chat.SayPacket(content), ct);

    /// <summary>
    /// Sends the given message to the family chat.
    /// </summary>
    /// <remarks>
    /// Should be used only if the user is in a family.
    /// </remarks>
    /// <param name="content">The content of the message.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendFamilyMessage(string content, CancellationToken ct = default)
        => _client.SendPacketAsync(":" + content, ct);

    /// <summary>
    /// Sends the given message to the group chat.
    /// </summary>
    /// <remarks>
    /// Should be used only if the user is in a group. (with people, not only pets).
    /// </remarks>
    /// <param name="content">The content of the message.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendGroupMessage(string content, CancellationToken ct = default)
        => _client.SendPacketAsync(";" + content, ct);

    /// <summary>
    /// Sends the given message to the target only.
    /// </summary>
    /// <remarks>
    /// Won't return if the whisper has actually came through, event has to be hooked
    /// up to know if the whisper has went through (and you can know only for messages that are sufficiently long).
    /// </remarks>
    /// <param name="targetName">The name of the user you want to whisper to.</param>
    /// <param name="content">The content of the message.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendWhisper(string targetName, string content, CancellationToken ct = default)
        => _client.SendPacketAsync($"/{targetName} {content}", ct);
}