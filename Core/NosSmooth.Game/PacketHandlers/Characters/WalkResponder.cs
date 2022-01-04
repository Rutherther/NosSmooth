//
//  WalkResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Packets.Packets.Client.Movement;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Characters;

/// <summary>
/// Responds to walk packet.
/// </summary>
public class WalkResponder : IPacketResponder<WalkPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public WalkResponder(Game game, EventDispatcher eventDispatcher)
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<WalkPacket> packetArgs, CancellationToken ct = default)
    {
        var character = _game.Character;
        var packet = packetArgs.Packet;
        if (character is not null && character.Position is not null)
        {
            var oldPosition = new Position
            {
                X = character.Position.X,
                Y = character.Position.Y
            };

            character.Position.X = packet.PositionX;
            character.Position.Y = packet.PositionY;

            return await _eventDispatcher.DispatchEvent
            (
                new MovedEvent(character, character.Id, oldPosition, character.Position),
                ct
            );
        }
        else if (character?.Position is null)
        {
            await _game.CreateOrUpdateCharacterAsync
            (
                () => new Character
                (
                    Position: new Position()
                    {
                        X = packet.PositionX,
                        Y = packet.PositionY
                    }
                ),
                (c) => c with
                {
                    Position = new Position()
                    {
                        X = packet.PositionX,
                        Y = packet.PositionY
                    }
                },
                ct: ct
            );
        }

        return Result.FromSuccess();
    }
}