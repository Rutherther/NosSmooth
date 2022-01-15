//
//  BoolStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using NosSmooth.PacketSerializer.Abstractions;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="bool"/>.
/// </summary>
public class BoolStringConverter : BaseStringConverter<bool>
{
    /// <inheritdoc />
    public override Result Serialize(bool obj, PacketStringBuilder builder)
    {
        builder.Append(obj ? '1' : '0');
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<bool> Deserialize(ref PacketStringEnumerator stringEnumerator)
    {
        var nextTokenResult = stringEnumerator.GetNextToken(out var packetToken);
        if (!nextTokenResult.IsSuccess)
        {
            return Result<bool>.FromError(nextTokenResult);
        }

        return packetToken.Token[0] == '1' ? true : false;
    }
}