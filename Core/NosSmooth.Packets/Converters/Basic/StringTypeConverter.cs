//
//  StringTypeConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Remora.Results;

namespace NosSmooth.Packets.Converters.Basic;

/// <summary>
/// Converter of <see cref="string"/>.
/// </summary>
public class StringTypeConverter : BasicTypeConverter<string>
{
    /// <inheritdoc />
    protected override Result<string?> Deserialize(string value)
    {
        return value;
    }
}