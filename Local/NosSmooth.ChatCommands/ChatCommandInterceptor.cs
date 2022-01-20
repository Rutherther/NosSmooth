//
//  ChatCommandInterceptor.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection.Emit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalClient;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Enums.Chat;
using NosSmooth.Packets.Server.Chat;
using Remora.Commands.Services;

namespace NosSmooth.ChatCommands;

/// <summary>
/// Handles commands in the chat.
/// </summary>
public class ChatCommandInterceptor : IPacketInterceptor
{
    private readonly CommandService _commandService;
    private readonly IServiceProvider _serviceProvider;
    private readonly FeedbackService _feedbackService;
    private readonly ILogger<ChatCommandInterceptor> _logger;
    private readonly ChatCommandsOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandInterceptor"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="commandService">The command service.</param>
    /// <param name="serviceProvider">The services.</param>
    /// <param name="feedbackService">The feedback service.</param>
    /// <param name="logger">The logger.</param>
    public ChatCommandInterceptor
    (
        IOptions<ChatCommandsOptions> options,
        CommandService commandService,
        IServiceProvider serviceProvider,
        FeedbackService feedbackService,
        ILogger<ChatCommandInterceptor> logger
    )
    {
        _commandService = commandService;
        _serviceProvider = serviceProvider;
        _feedbackService = feedbackService;
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc />
    public bool InterceptSend(ref string packet)
    {
        ReadOnlySpan<char> span = packet;
        if (span.StartsWith("say ") && span.Slice(4).StartsWith(_options.Prefix))
        {
            var command = span.Slice(4 + _options.Prefix.Length).ToString();
            Task.Run(async () => await ExecuteCommand(command));
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool InterceptReceive(ref string packet)
    {
        return true;
    }

    private async Task ExecuteCommand(string command)
    {
        var preparedResult = await _commandService.TryPrepareCommandAsync(command, _serviceProvider);
        if (!preparedResult.IsSuccess)
        {
            _logger.LogError($"Could not prepare \"{command}\"");
            _logger.LogResultError(preparedResult);
            await _feedbackService.SendErrorMessageAsync($"Could not prepare the given command. {preparedResult.Error.Message}");
        }

        var executeResult = await _commandService.TryExecuteAsync(preparedResult.Entity, _serviceProvider);
        if (!executeResult.IsSuccess)
        {
            _logger.LogError($"Could not execute \"{command}\"");
            _logger.LogResultError(executeResult);
            await _feedbackService.SendErrorMessageAsync($"Could not execute the given command. {executeResult.Error.Message}");
        }
    }
}