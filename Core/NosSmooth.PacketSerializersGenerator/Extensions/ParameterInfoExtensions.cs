//
//  ParameterInfoExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;

namespace NosSmooth.PacketSerializersGenerator.Extensions;

/// <summary>
/// Extensions for <see cref="ParameterInfo"/>.
/// </summary>
public static class ParameterInfoExtensions
{
    /// <summary>
    /// Gets the name of the error variable.
    /// </summary>
    /// <param name="parameterInfo">The parameter.</param>
    /// <returns>The name of the error variable.</returns>
    public static string GetErrorVariableName(this ParameterInfo parameterInfo)
    {
        return $"{parameterInfo.Name}Error";
    }

    /// <summary>
    /// Gets the name of the error variable.
    /// </summary>
    /// <param name="parameterInfo">The parameter.</param>
    /// <returns>The name of the error variable.</returns>
    public static string GetResultVariableName(this ParameterInfo parameterInfo)
    {
        return $"{parameterInfo.Name}Result";
    }

    /// <summary>
    /// Gets the name of the error variable.
    /// </summary>
    /// <param name="parameterInfo">The parameter.</param>
    /// <returns>The name of the error variable.</returns>
    public static string GetVariableName(this ParameterInfo parameterInfo)
    {
        return parameterInfo.Name;
    }

    /// <summary>
    /// Gets the name of the nullable variable.
    /// </summary>
    /// <param name="parameterInfo">The parameter.</param>
    /// <returns>The name of the nullable variable.</returns>
    public static string GetNullableVariableName(this ParameterInfo parameterInfo)
    {
        return $"{parameterInfo.Name}Nullable";
    }

    /// <summary>
    /// Gets the type of the parameter as nullable.
    /// </summary>
    /// <param name="parameterInfo">The parameter.</param>
    /// <returns>The nullable type.</returns>
    public static string GetNullableType(this ParameterInfo parameterInfo)
    {
        return parameterInfo.Type.ToString().TrimEnd('?') + "?";
    }

    /// <summary>
    /// Gets the type of the parameter with ? if the parameter is nullable..
    /// </summary>
    /// <param name="parameterInfo">The parameter.</param>
    /// <returns>The type.</returns>
    public static string GetActualType(this ParameterInfo parameterInfo)
    {
        return parameterInfo.Type.ToString().TrimEnd('?') + (parameterInfo.Nullable ? "?" : string.Empty);
    }

    /// <summary>
    /// Gets whether the parameter is marked as optional.
    /// </summary>
    /// <param name="parameterInfo">The parameter info.</param>
    /// <returns>Whether the parameter is optional.</returns>
    public static bool IsOptional(this ParameterInfo parameterInfo)
    {
        return parameterInfo.Attributes.First().GetNamedValue("IsOptional", false);
    }
}