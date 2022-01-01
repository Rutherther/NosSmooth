//
//  UpgradeRareSubPacketConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using NosSmooth.Packets.Packets.Server.Weapons;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Packets;

/// <summary>
/// Converter for <see cref="UpgradeRareSubPacket"/>.
/// </summary>
public class UpgradeRareSubPacketConverter : BaseTypeConverter<UpgradeRareSubPacket>
{
    /// <inheritdoc />
    public override Result Serialize(UpgradeRareSubPacket? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append("-");
            return Result.FromSuccess();
        }
        builder.Append($"{obj.Upgrade}{obj.Rare}");
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<UpgradeRareSubPacket?> Deserialize(PacketStringEnumerator stringEnumerator)
    {
        var tokenResult = stringEnumerator.GetNextToken();
        if (!tokenResult.IsSuccess)
        {
            return Result<UpgradeRareSubPacket?>.FromError(tokenResult);
        }

        var token = tokenResult.Entity.Token;
        if (token.Length > 3)
        {
            return new CouldNotConvertError(this, token, "The string is not two/three characters long.");
        }

        var upgradeString = token.Substring(0, token.Length - 1);
        var rareString = token[token.Length - 1].ToString();

        if (!byte.TryParse(upgradeString, out var upgrade))
        {
            return new CouldNotConvertError(this, upgradeString, "Could not parse as byte");
        }

        if (!sbyte.TryParse(rareString, out var rare))
        {
            return new CouldNotConvertError(this, rareString, "Could not parse as byte");
        }

        return new UpgradeRareSubPacket(upgrade, rare);
    }
}