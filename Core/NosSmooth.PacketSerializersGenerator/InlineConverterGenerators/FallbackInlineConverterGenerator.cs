//
//  FallbackInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <inheritdoc />
public class FallbackInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
    {
        return true;
    }

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        textWriter.WriteMultiline
        (
            $@"
var {parameter.GetResultVariableName()} = _typeConverterRepository.Serialize<{parameter.GetActualType()}>(obj.{parameter.Name}, builder);
if (!{parameter.GetResultVariableName()}.IsSuccess)
{{
    return Result.FromError(new PacketParameterSerializerError(this, ""{parameter.Name}"", {parameter.GetResultVariableName()}), {parameter.GetResultVariableName()});
}}
"
        );
        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        string isLastString = packet.Parameters.IsLast ? "true" : "false";
        textWriter.WriteMultiline($@"
var {parameter.GetResultVariableName()} = _typeConverterRepository.Deserialize<{parameter.GetNullableType()}>(stringEnumerator);
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", stringEnumerator, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{packet.Name}?>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
var {parameter.GetNullableVariableName()} = {parameter.GetResultVariableName()}.Entity;
");
        return null;
    }
}