//
//  SkillsReceivedResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game;
using NosSmooth.Game.Apis;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using Remora.Results;

namespace SimplePiiBot.Responders;

/// <inheritdoc />
public class SkillsReceivedResponder : IGameResponder<SkillsReceivedEvent>
{
    private readonly NostaleChatPacketApi _chatApi;
    private readonly Game _theGame;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkillsReceivedResponder"/> class.
    /// </summary>
    /// <param name="chatApi">The chat packet api.</param>
    /// <param name="theGame">The game.</param>
    public SkillsReceivedResponder(NostaleChatPacketApi chatApi, Game theGame)
    {
        _chatApi = chatApi;
        _theGame = theGame;
    }

    /// <inheritdoc />
    public Task<Result> Respond(SkillsReceivedEvent gameEvent, CancellationToken ct = default)
    {
        return _chatApi.ReceiveSystemMessageAsync("Received the skills, the bot will function properly.", ct: ct);
    }
}