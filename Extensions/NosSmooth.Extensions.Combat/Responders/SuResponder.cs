//
//  SuResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Battle;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Responders;

/// <summary>
/// Responds to su packet.
/// </summary>
public class SuResponder : IPacketResponder<SuPacket>
{
    private readonly CombatManager _combatManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuResponder"/> class.
    /// </summary>
    /// <param name="combatManager">The combat manager.</param>
    public SuResponder(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<SuPacket> packetArgs, CancellationToken ct = default)
    {
        _combatManager.CancelSkillTokensAsync(ct);
        return Task.FromResult(Result.FromSuccess());
    }
}