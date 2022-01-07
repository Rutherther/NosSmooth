//
//  ShortStringConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="short"/>.
/// </summary>
public class ShortStringConverter : BasicTypeConverter<short>
{
    /// <inheritdoc />
    protected override Result<short> Deserialize(string value)
    {
        if (!short.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value, "Could not parse as short.");
        }

        return parsed;
    }
}