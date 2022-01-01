//
//  BoolInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <inheritdoc />
public class BoolInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Parameter.Type!.ToString() == "bool";

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        if (parameter.Nullable)
        {
            textWriter.WriteLine($"if (obj.{parameter.Name} is null)");
            textWriter.WriteLine("{");
            textWriter.Indent++;
            textWriter.WriteLine("builder.Append('-');");
            textWriter.Indent--;
            textWriter.WriteLine("}");
            textWriter.WriteLine("else");
        }
        textWriter.WriteLine("{");
        textWriter.Indent++;
        textWriter.WriteLine($"builder.Append(obj.{parameter.Name} ? '1' : '0');");
        textWriter.Indent--;
        textWriter.WriteLine("}");
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        textWriter.WriteLine($"{Constants.HelperClass}.ParseBool(stringEnumerator);");
        /*string isLastString = packet.Parameters.IsLast ? "true" : "false";
        textWriter.WriteMultiline($@"
var {parameter.GetResultVariableName()} = stringEnumerator.GetNextToken();
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", stringEnumerator, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{packet.Name}?>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
{parameter.GetNullableType()} {parameter.GetNullableVariableName()} = {parameter.GetResultVariableName()}.Entity.Token == ""1"" ? true : ({parameter.GetResultVariableName()}.Entity.Token == ""-"" ? null : false);
");*/
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        textWriter.WriteLine(@"
public static Result<bool?> ParseBool(PacketStringEnumerator stringEnumerator)
{{
    var tokenResult = stringEnumerator.GetNextToken();
    if (!tokenResult.IsSuccess)
    {{
        return Result<bool?>.FromError(tokenResult);
    }}

    var token = tokenResult.Entity.Token;
    if (token == ""-"")
    {{
        return Result<bool?>.FromSuccess(null);
    }}

    return token == ""1"" ? true : false;
}}
");
    }
}