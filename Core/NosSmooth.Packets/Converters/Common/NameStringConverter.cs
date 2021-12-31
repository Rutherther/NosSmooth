//
//  NameStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Common;
using NosSmooth.Packets.Converters.Basic;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Common;

/// <summary>
/// Converter of <see cref="NameString"/>.
/// </summary>
public class NameStringConverter : BaseTypeConverter<NameString>
{
    /// <inheritdoc />
    public override Result Serialize(NameString? obj, PacketStringBuilder builder)
    {
        if (obj is null)
        {
            builder.Append("-");
            return Result.FromSuccess();
        }

        builder.Append(obj.PacketName);
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public override Result<NameString?> Deserialize(PacketStringEnumerator stringEnumerator)
    {
        var tokenResult = stringEnumerator.GetNextToken();
        if (!tokenResult.IsSuccess)
        {
            return Result<NameString?>.FromError(tokenResult);
        }

        return NameString.FromPacket(tokenResult.Entity.Token);
    }
}