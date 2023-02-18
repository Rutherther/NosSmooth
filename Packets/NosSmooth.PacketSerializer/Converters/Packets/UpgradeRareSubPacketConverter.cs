//
//  UpgradeRareSubPacketConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Server.Weapons;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Converters.Packets;

/// <summary>
/// Converter for <see cref="UpgradeRareSubPacket"/>.
/// </summary>
public class UpgradeRareSubPacketConverter : BaseStringConverter<UpgradeRareSubPacket>
{
    /// <inheritdoc />
    public override Result Serialize(UpgradeRareSubPacket? obj, in PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append('-');
            return Result.FromSuccess();
        }
        var rare = obj.Rare != 0 ? obj.Rare.ToString() : string.Empty;
        builder.Append($"{obj.Upgrade}{rare}");
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<UpgradeRareSubPacket?> Deserialize(in PacketStringEnumerator stringEnumerator, DeserializeOptions options)
    {
        var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
        if (!tokenResult.IsSuccess)
        {
            return Result<UpgradeRareSubPacket?>.FromError(tokenResult);
        }
        var token = packetToken.Token;

        if (token.Length == 1 && token.StartsWith("-"))
        {
            return Result<UpgradeRareSubPacket?>.FromSuccess(null);
        }

        if (token.Length > 3)
        {
            return new CouldNotConvertError(this, token.ToString(), "The string is not two/three characters long.");
        }

        if (token.Length == 2 && token.StartsWith("10"))
        {
            return Result<UpgradeRareSubPacket?>.FromSuccess(new UpgradeRareSubPacket(10, 0));
        }

        if (token.Length == 2 && token.StartsWith("-1"))
        {
            return Result<UpgradeRareSubPacket?>.FromSuccess(null);
        }

        var upgradeString = token.Slice(0, Math.Max(1, token.Length - 1));
        var rareString = token.Slice(token.Length - 1);

        if (!byte.TryParse(upgradeString, out var upgrade))
        {
            return new CouldNotConvertError(this, upgradeString.ToString(), "Could not parse as byte");
        }

        sbyte rare = 0;
        if (token.Length > 1 && !sbyte.TryParse(rareString, out rare))
        {
            return new CouldNotConvertError(this, rareString.ToString(), "Could not parse as byte");
        }

        return new UpgradeRareSubPacket(upgrade, rare);
    }
}