//
//  ULongTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="ulong"/>.
/// </summary>
public class ULongTypeConverter : BasicTypeConverter<ulong>
{
    /// <inheritdoc />
    protected override Result<ulong> Deserialize(string value)
    {
        if (!ulong.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value, "Could not parse as an ulong.");
        }

        return parsed;
    }
}