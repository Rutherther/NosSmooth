//
//  UpgradeRareSubPacketConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    public override Result Serialize(UpgradeRareSubPacket? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append('-');
            return Result.FromSuccess();
        }
        builder.Append($"{obj.Upgrade}{obj.Rare}");
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<UpgradeRareSubPacket?> Deserialize(ref PacketStringEnumerator stringEnumerator)
    {
        var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
        if (!tokenResult.IsSuccess)
        {
            return Result<UpgradeRareSubPacket?>.FromError(tokenResult);
        }
        var token = packetToken.Token;

        if (token.Length > 3)
        {
            return new CouldNotConvertError(this, token.ToString(), "The string is not two/three characters long.");
        }

        if (token.Length == 1 && token[0] == '0')
        {
            return Result<UpgradeRareSubPacket?>.FromSuccess(new UpgradeRareSubPacket(0, 0));
        }

        var upgradeString = token.Slice(0, token.Length - 1);
        var rareString = token.Slice(token.Length - 1);

        if (!byte.TryParse(upgradeString, out var upgrade))
        {
            return new CouldNotConvertError(this, upgradeString.ToString(), "Could not parse as byte");
        }

        if (!sbyte.TryParse(rareString, out var rare))
        {
            return new CouldNotConvertError(this, rareString.ToString(), "Could not parse as byte");
        }

        return new UpgradeRareSubPacket(upgrade, rare);
    }
}