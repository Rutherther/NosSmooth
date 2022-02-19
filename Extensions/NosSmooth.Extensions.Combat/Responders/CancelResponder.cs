//
//  CancelResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Extensions.Combat.Responders;

/// <summary>
/// Responds to cancel packet.
/// </summary>
public class CancelResponder : IPacketResponder<CancelPacket>
{
    private readonly CombatManager _combatManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelResponder"/> class.
    /// </summary>
    /// <param name="combatManager">The combat manager.</param>
    public CancelResponder(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<CancelPacket> packetArgs, CancellationToken ct = default)
    {
        _combatManager.CancelSkillTokensAsync(ct);
        return Task.FromResult(Result.FromSuccess());
    }
}