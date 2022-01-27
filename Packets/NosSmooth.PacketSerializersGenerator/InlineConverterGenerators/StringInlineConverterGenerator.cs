//
//  StringInlineConverterGenerator.cs
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
public class StringInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
        => typeSyntax?.ToString().TrimEnd('?') == "string" || typeSymbol?.ToString().TrimEnd('?') == "string";

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, string variableName, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
    {
        textWriter.WriteLine($"builder.Append({variableName} ?? \"-\");");
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
    {
        textWriter.WriteLine($"{Constants.HelperClass}.ParseString(ref stringEnumerator);");
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        textWriter.WriteLine
        (
            @"
public static Result<string?> ParseString(ref PacketStringEnumerator stringEnumerator)
{{
    var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
    if (!tokenResult.IsSuccess)
    {{
        return Result<string?>.FromError(tokenResult);
    }}

    if (packetToken.Length == 1 && packetToken.Token[0] == '-')
    {{
        return Result<string?>.FromSuccess(null);
    }}

    return packetToken.Token.ToString();
}}
"
        );
    }
}
