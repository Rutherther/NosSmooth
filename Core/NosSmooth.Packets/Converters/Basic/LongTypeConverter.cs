//
//  LongTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="long"/>.
/// </summary>
public class LongTypeConverter : BasicTypeConverter<long>
{
    /// <inheritdoc />
    protected override Result<long> Deserialize(string value)
    {
        if (!long.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value, "Could not parse as an long.");
        }

        return parsed;
    }
}