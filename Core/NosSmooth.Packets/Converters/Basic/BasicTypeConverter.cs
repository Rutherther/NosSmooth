//
//  BasicTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Basic type converter for converting using <see cref="Convert"/>.
/// </summary>
/// <typeparam name="TBasicType">The basic type, that contains correct to string.</typeparam>
public abstract class BasicTypeConverter<TBasicType> : BaseStringConverter<TBasicType>
{
    /// <inheritdoc />
    public override Result Serialize(TBasicType? obj, PacketStringBuilder builder)
    {
        builder.Append(obj?.ToString() ?? "-");
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<TBasicType?> Deserialize(ref PacketStringEnumerator stringEnumerator)
    {
        var nextTokenResult = stringEnumerator.GetNextToken(out var packetToken);
        if (!nextTokenResult.IsSuccess)
        {
            return Result<TBasicType?>.FromError(nextTokenResult);
        }

        if (packetToken.Token[0] == '-' && packetToken.Token.Length == 1)
        {
            return Result<TBasicType?>.FromSuccess(default);
        }

        return Deserialize(packetToken.Token);
    }

    /// <summary>
    /// Deserialize the given string value.
    /// </summary>
    /// <param name="value">The value to deserialize.</param>
    /// <returns>The deserialized value or an error.</returns>
    protected abstract Result<TBasicType?> Deserialize(ReadOnlySpan<char> value);
}