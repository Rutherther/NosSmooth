//
//  StringInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <inheritdoc />
public class StringInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Parameter.Type!.ToString() == "string";

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        textWriter.WriteLine($"builder.Append(obj.{parameter.Name} ?? \"-\");");
        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        string isLastString = packet.Parameters.IsLast ? "true" : "false";
        textWriter.WriteMultiline($@"
var {parameter.GetResultVariableName()} = stringEnumerator.GetNextToken();
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", stringEnumerator, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{parameter.GetActualType()}>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
var {parameter.GetNullableVariableName()} = {parameter.GetResultVariableName()}.Entity.Token == ""-"" ? null : {parameter.GetResultVariableName()}.Entity.Token;
");
        return null;
    }
}