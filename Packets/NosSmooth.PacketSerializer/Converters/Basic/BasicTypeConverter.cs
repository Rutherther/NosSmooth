//
//  BasicTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.PacketSerializer.Abstractions;
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
        builder.Append(obj?.ToString() ?? GetNullSymbol());
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

        var nullSymbol = GetNullSymbol();
        if (packetToken.Token.Length == nullSymbol.Length && packetToken.Token.StartsWith(nullSymbol))
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

    /// <summary>
    /// Gets the symbol that represents null.
    /// </summary>
    /// <returns>The null symbol.</returns>
    protected virtual string GetNullSymbol()
    {
        return "-1";
    }
}