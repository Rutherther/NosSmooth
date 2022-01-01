//
//  ByteTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="byte"/>.
/// </summary>
public class ByteTypeConverter : BasicTypeConverter<byte>
{
    /// <inheritdoc />
    protected override Result<byte> Deserialize(string value)
    {
        if (!byte.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value, "Could not parse as an byte.");
        }

        return parsed;
    }
}