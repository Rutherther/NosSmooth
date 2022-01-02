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
        textWriter.WriteLine($"{Constants.HelperClass}.ParseString(stringEnumerator);");
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        textWriter.WriteLine
        (
            @"
public static Result<string?> ParseString(PacketStringEnumerator stringEnumerator)
{{
    var tokenResult = stringEnumerator.GetNextToken();
    if (!tokenResult.IsSuccess)
    {{
        return Result<string?>.FromError(tokenResult);
    }}

    return tokenResult.Entity.Token;
}}
"
        );
    }
}