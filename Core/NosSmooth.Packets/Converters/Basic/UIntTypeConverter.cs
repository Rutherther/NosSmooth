//
//  UIntTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="uint"/>.
/// </summary>
public class UIntTypeConverter : BasicTypeConverter<uint>
{
    /// <inheritdoc />
    protected override Result<uint> Deserialize(string value)
    {
        if (!uint.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value, "Could not parse as uint");
        }

        return parsed;
    }
}