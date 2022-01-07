//
//  BoolStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
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
    public override Result<bool> Deserialize(PacketStringEnumerator stringEnumerator)
    {
        var nextTokenResult = stringEnumerator.GetNextToken();
        if (!nextTokenResult.IsSuccess)
        {
            return Result<bool>.FromError(nextTokenResult);
        }

        if (nextTokenResult.Entity.Token == "-")
        {
            return Result<bool>.FromSuccess(default);
        }

        return nextTokenResult.Entity.Token == "1" ? true : false;
    }
}