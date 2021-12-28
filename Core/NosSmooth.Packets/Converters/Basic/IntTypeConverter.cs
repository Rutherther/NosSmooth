//
//  IntTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="int"/>.
/// </summary>
public class IntTypeConverter : BasicTypeConverter<int>
{
    /// <inheritdoc />
    protected override Result<int> Deserialize(string value)
    {
        if (!int.TryParse(value, out var parsed))
        {
            return new CouldNotConvertError(this, value, "Could not parse as int.");
        }

        return parsed;
    }
}