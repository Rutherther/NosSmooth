//
//  NameStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Converters.Basic;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Common;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Common;

/// <summary>
/// Converter of <see cref="NameString"/>.
/// </summary>
public class NameStringConverter : BaseStringConverter<NameString>
{
    /// <inheritdoc />
    public override Result Serialize(NameString? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append('-');
            return Result.FromSuccess();
        }

        builder.Append(obj.PacketName);
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<NameString?> Deserialize(ref PacketStringEnumerator stringEnumerator)
    {
        var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
        if (!tokenResult.IsSuccess)
        {
            return Result<NameString?>.FromError(tokenResult);
        }

        if (packetToken.Token[0] == '-' && packetToken.Token.Length == 1)
        {
            return Result<NameString?>.FromSuccess(null);
        }

        return NameString.FromPacket(packetToken.Token.ToString());
    }
}