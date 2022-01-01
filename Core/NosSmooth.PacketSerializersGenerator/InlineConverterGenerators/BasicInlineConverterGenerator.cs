//
//  BasicInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <summary>
/// Serializes and deserializes
/// long, ulong, int, uint, short, ushort, byte, sbyte.
/// </summary>
public class BasicInlineConverterGenerator : IInlineConverterGenerator
{
    /// <summary>
    /// The list of the types to handle.
    /// </summary>
    public static IReadOnlyList<string> HandleTypes => new[] { "long", "ulong", "int", "uint", "short", "ushort", "byte", "sbyte" };

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => HandleTypes.Contains(parameter.Parameter.Type!.ToString());

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        textWriter.WriteLine
            ($"builder.Append(obj.{parameter.Name}{(parameter.Nullable ? "?" : string.Empty)}.ToString() ?? \"-\");");
        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        var type = parameter.Parameter.Type!.ToString();
        string isLastString = packet.Parameters.IsLast ? "true" : "false";
        textWriter.WriteMultiline
        (
            $@"
var {parameter.GetResultVariableName()} = stringEnumerator.GetNextToken();
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", stringEnumerator, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{packet.Name}?>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
{parameter.GetVariableName()} = default;
{parameter.GetNullableType()} {parameter.GetNullableVariableName()};
if ({parameter.GetResultVariableName()}.Entity.Token == ""-"") {{
    {parameter.GetNullableVariableName()} = null;
}}
else if (!{type}.TryParse({parameter.GetResultVariableName()}.Entity.Token, out {parameter.GetVariableName()})) {{
    return new PacketParameterSerializerError(this, ""{parameter.Name}"", {parameter.GetResultVariableName()}, $""Could not convert {{{parameter.GetResultVariableName()}.Entity.Token}} as {type} in inline converter"");
}}
{parameter.GetNullableVariableName()} = {parameter.GetVariableName()};
"
        );
        return null;
    }
}