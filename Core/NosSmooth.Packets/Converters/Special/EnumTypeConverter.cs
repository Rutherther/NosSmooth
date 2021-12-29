//
//  EnumTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Special;

/// <summary>
/// Converts all enums.
/// </summary>
public class EnumTypeConverter : ISpecialTypeConverter
{
    /// <inheritdoc />
    public bool ShouldHandle(Type type)
        => type.IsEnum;

    /// <inheritdoc />
    public Result<object?> Deserialize(Type type, PacketStringEnumerator stringEnumerator)
    {
        var tokenResult = stringEnumerator.GetNextToken();
        if (!tokenResult.IsSuccess)
        {
            return Result.FromError(tokenResult);
        }

        return Enum.Parse(type, tokenResult.Entity.Token);
    }

    /// <inheritdoc />
    public Result Serialize(Type type, object? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append("-");
            return Result.FromSuccess();
        }

        builder.Append(Convert.ToInt64(obj).ToString());
        return Result.FromSuccess();
    }
}