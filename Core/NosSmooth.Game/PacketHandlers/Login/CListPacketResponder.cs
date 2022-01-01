//
//  CListPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Login;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Login;

/// <summary>
/// Handles FStashEnd packet to remove game data.
/// </summary>
public class CListPacketResponder : IPacketResponder<ClistPacket>
{
    private readonly EventDispatcher _eventDispatcher;
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="CListPacketResponder"/> class.
    /// </summary>
    /// <param name="eventDispatcher">The events dispatcher.</param>
    /// <param name="game">The nostale game.</param>
    public CListPacketResponder(EventDispatcher eventDispatcher, Game game)
    {
        _eventDispatcher = eventDispatcher;
        _game = game;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<ClistPacket> packetArgs, CancellationToken ct = default)
    {
        await _game.SetSemaphore.WaitAsync(ct);
        bool logout = _game.Character is not null || _game.CurrentMap is not null;
        _game.Character = null;
        _game.CurrentMap = null;
        _game.CurrentRaid = null;
        _game.SetSemaphore.Release();

        if (logout)
        {
            return await _eventDispatcher.DispatchEvent(new LogoutEvent(), ct);
        }

        return Result.FromSuccess();
    }
}