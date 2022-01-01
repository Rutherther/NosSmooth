//
//  ParameterChecker.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Helpers for checking packet parameters.
/// </summary>
public static class ParameterChecker
{
    /// <summary>
    /// Checks that the given parameter has exactly one packet attribute.
    /// </summary>
    /// <param name="packetInfo">The packet.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns>An error, if any.</returns>
    public static IError? CheckHasOneAttribute(PacketInfo packetInfo, ParameterInfo parameter)
    {
        if (parameter.Attributes.Count > 1)
        {
            return new DiagnosticError
            (
                "SGAttr",
                "Packet constructor parameter with multiple packet attributes",
                "Found multiple packet attributes on parameter {0} in {1}. That is not supported for this type of packet attribute.",
                parameter.Parameter.SyntaxTree,
                parameter.Parameter.FullSpan,
                new List<object?>
                (
                    new[]
                    {
                        parameter.Name,
                        packetInfo.Name
                    }
                )
            );
        }

        return null;
    }

    /// <summary>
    /// Checks that the given parameter is nullable, if it is optional.
    /// </summary>
    /// <param name="packetInfo">The packet.</param>
    /// <param name="parameter">The parameter.</param>
    /// <returns>An error, if any.</returns>
    public static IError? CheckOptionalIsNullable(PacketInfo packetInfo, ParameterInfo parameter)
    {
        if (parameter.IsOptional() && !parameter.Nullable)
        {
            return new DiagnosticError
            (
                "SGNull",
                "Optional parameters must be nullable",
                "The parameter {0} in {1} has to be nullable, because it is optional.",
                parameter.Parameter.SyntaxTree,
                parameter.Parameter.FullSpan,
                new List<object?>(new[] { parameter.Name, packetInfo.Name })
            );
        }

        return null;
    }
}