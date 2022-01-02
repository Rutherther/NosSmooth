//
//  EnumInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <inheritdoc />
public class EnumInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Type.TypeKind == TypeKind.Enum;

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        var underlyingType = ((INamedTypeSymbol)parameter.Type).EnumUnderlyingType!.ToString();
        if (parameter.Nullable)
        {
            textWriter.WriteLine("if (obj is null)");
            textWriter.WriteLine("{");
            textWriter.WriteLine("builder.Append('-');");
            textWriter.WriteLine("}");
            textWriter.WriteLine("else");
        }
        textWriter.WriteLine("{");
        textWriter.WriteLine
            ($"builder.Append(({underlyingType})obj.{parameter.Name});");
        textWriter.WriteLine("}");

        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        var underlyingType = ((INamedTypeSymbol)parameter.Type).EnumUnderlyingType!.ToString();
        string isLastString = packet.Parameters.IsLast ? "true" : "false";
        textWriter.WriteMultiline
        (
            $@"
var {parameter.GetResultVariableName()} = stringEnumerator.GetNextToken();
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", stringEnumerator, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{parameter.GetActualType()}>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
{parameter.GetVariableName()} = default;
{underlyingType} {parameter.GetVariableName()}Underlying = default;
{parameter.GetNullableType()} {parameter.GetNullableVariableName()};
if ({parameter.GetResultVariableName()}.Entity.Token == ""-"") {{
    {parameter.GetNullableVariableName()} = null;
}}
else if (!{underlyingType}.TryParse({parameter.GetResultVariableName()}.Entity.Token, out {parameter.GetVariableName()}Underlying)) {{
    return new PacketParameterSerializerError(this, ""{parameter.Name}"", {parameter.GetResultVariableName()}, $""Could not convert {{{parameter.GetResultVariableName()}.Entity.Token}} as {underlyingType} in inline converter"");
}}
{parameter.GetNullableVariableName()} = ({parameter.GetNullableType()}){parameter.GetVariableName()}Underlying;
"
        );
        return null;
    }
}