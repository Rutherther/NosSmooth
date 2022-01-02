//
//  EnumInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
        => typeSymbol?.TypeKind == TypeKind.Enum;

    /// <inheritdoc />
    public IError? GenerateSerializerPart
    (
        IndentedTextWriter textWriter,
        string variableName,
        TypeSyntax? typeSyntax,
        ITypeSymbol? typeSymbol
    )
    {
        var underlyingType = ((INamedTypeSymbol)typeSymbol!).EnumUnderlyingType!.ToString();
        if ((typeSyntax?.IsNullable() ?? false) || (typeSymbol?.IsNullable() ?? false))
        {
            textWriter.WriteLine("if (obj is null)");
            textWriter.WriteLine("{");
            textWriter.WriteLine("builder.Append('-');");
            textWriter.WriteLine("}");
            textWriter.WriteLine("else");
        }
        textWriter.WriteLine("{");
        textWriter.WriteLine
            ($"builder.Append(({underlyingType}){variableName});");
        textWriter.WriteLine("}");

        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
    {
        if (_enumTypes.All(x => x.ToString() != typeSymbol!.ToString()))
        {
            _enumTypes.Add(typeSymbol!);
        }

        textWriter.WriteLine
        (
            $"{Constants.HelperClass}.ParseEnum{typeSymbol?.ToString().TrimEnd('?').Replace('.', '_')}(typeConverter, ref stringEnumerator);"
        );
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
public static Result<{type}?> ParseEnum{type.ToString().Replace('.', '_')}(IStringConverter typeConverter, ref PacketStringEnumerator stringEnumerator)
{{
    var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
    if (!tokenResult.IsSuccess)
    {{
        return Result<{type}?>.FromError(tokenResult);
    }}

    var token = packetToken.Token;
    if (token[0] == '-')
    {{
        return Result<{type}?>.FromSuccess(null);
    }}

    if (!{underlyingType}.TryParse(token, out var val))
    {{
        return new CouldNotConvertError(typeConverter, token.ToString(), ""Could not convert as {type} in inline converter"");
    }}

    return ({type}?)val;
}}
"
            );
        }
    }
}
