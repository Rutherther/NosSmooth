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
    private readonly List<ITypeSymbol> _enumTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumInlineConverterGenerator"/> class.
    /// </summary>
    public EnumInlineConverterGenerator()
    {
        _enumTypes = new List<ITypeSymbol>();
    }

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Type.TypeKind == TypeKind.Enum;

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        var underlyingType = ((INamedTypeSymbol)parameter.Type).EnumUnderlyingType!.ToString();
        textWriter.WriteLine
        (
            $"builder.Append((({underlyingType}?)obj.{parameter.Name}{(parameter.Nullable ? "?" : string.Empty)}).ToString() ?? \"-\");"
        );
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        if (_enumTypes.All(x => x.ToString() != parameter.Type.ToString()))
        {
            _enumTypes.Add(parameter.Type);
        }

        textWriter.WriteLine
            ($"{Constants.HelperClass}.ParseEnum{parameter.GetActualType().Replace('.', '_')}(this, stringEnumerator);");
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        foreach (var type in _enumTypes)
        {
            var underlyingType = ((INamedTypeSymbol)type).EnumUnderlyingType!.ToString();
            textWriter.WriteMultiline
            (
                $@"
public static Result<{type}?> ParseEnum{type.ToString().Replace('.', '_')}(ITypeConverter typeConverter, PacketStringEnumerator stringEnumerator)
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

    if (!{underlyingType}.TryParse(token, out var val))
    {{
        return new CouldNotConvertError(typeConverter, token, ""Could not convert as {type} in inline converter"");
    }}

    return ({type}?)val;
}}
"
            );
        }
    }
}