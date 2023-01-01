//
//  SkillUseResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using NosSmooth.Packets.Server.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Responders;

/// <summary>
/// Responds to su packet.
/// </summary>
public class SuResponder : IPacketResponder<SuPacket>, IPacketResponder<BsPacket>
{
    private readonly CombatManager _combatManager;
    private readonly Game.Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuResponder"/> class.
    /// </summary>
    /// <param name="combatManager">The combat manager.</param>
    /// <param name="game">The game.</param>
    public SuResponder(CombatManager combatManager, Game.Game game)
    {
        _combatManager = combatManager;
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<SuPacket> packetArgs, CancellationToken ct = default)
    {
        if (packetArgs.Packet.CasterEntityId == _game.Character?.Id)
        {
            _combatManager.CancelSkillTokensAsync(ct);
        }
        return Task.FromResult(Result.FromSuccess());
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<BsPacket> packetArgs, CancellationToken ct = default)
    {
        if (packetArgs.Packet.CasterEntityId == _game.Character?.Id)
        {
            _combatManager.CancelSkillTokensAsync(ct);
        }
        return Task.FromResult(Result.FromSuccess());
    }
}