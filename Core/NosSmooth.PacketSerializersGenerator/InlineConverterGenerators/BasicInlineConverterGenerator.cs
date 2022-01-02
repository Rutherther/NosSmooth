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
    public IError? CallDeserialize(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        var type = parameter.Parameter.Type!.ToString().Trim('?');
        textWriter.WriteLine($"{Constants.HelperClass}.ParseBasic{type}(this, stringEnumerator);");
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        foreach (var type in HandleTypes)
        {
            textWriter.WriteMultiline($@"
public static Result<{type}?> ParseBasic{type}(ITypeConverter typeConverter, PacketStringEnumerator stringEnumerator)
{{
    var tokenResult = stringEnumerator.GetNextToken();
    if (!tokenResult.IsSuccess)
    {{
        return Result<{type}?>.FromError(tokenResult);
    }}

    var token = tokenResult.Entity.Token;
    if (token == ""-"")
    {{
        return Result<{type}?>.FromSuccess(null);
    }}

    if (!{type}.TryParse(token, out var val))
    {{
        return new CouldNotConvertError(typeConverter, token, ""Could not convert as {type} in inline converter"");
    }}

    return val;
}}
");
        }
    }
}