//
//  EqResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Helpers;
using NosSmooth.Packets.Server.Inventory;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Entities;

/// <summary>
/// Responder to eq packet.
/// </summary>
public class EqResponder : IPacketResponder<EqPacket>
{
    private readonly Game _game;
    private readonly IInfoService _infoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EqResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="infoService">The info service.</param>
    public EqResponder(Game game, IInfoService infoService)
    {
        _game = game;
        _infoService = infoService;

    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<EqPacket> packetArgs, CancellationToken ct = default)
    {
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Result.FromSuccess();
        }

        var packet = packetArgs.Packet;
        var entity = map.Entities.GetEntity<Player>(packet.CharacterId);

        if (entity is null)
        {
            return Result.FromSuccess();
        }

        entity.Sex = packet.Sex;
        entity.Class = packet.Class;
        if (packet.Size is not null)
        {
            entity.Size = packet.Size.Value;
        }
        entity.Authority = packet.AuthorityType;
        entity.HairColor = packet.HairColor;
        entity.HairStyle = packet.HairStyle;
        entity.Equipment = await EquipmentHelpers.CreateEquipmentFromInSubpacketAsync
        (
            _infoService,
            packet.EquipmentSubPacket,
            packet.WeaponUpgradeRareSubPacket,
            packet.ArmorUpgradeRareSubPacket,
            ct
        );

        return Result.FromSuccess();
    }
}