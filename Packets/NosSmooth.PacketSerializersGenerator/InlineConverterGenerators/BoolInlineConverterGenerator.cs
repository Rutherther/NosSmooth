//
//  BoolInlineConverterGenerator.cs
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
public class BoolInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
        => typeSyntax?.ToString().TrimEnd('?') == "bool" || typeSymbol?.ToString().TrimEnd('?') == "bool";

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, string variableName, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
    {
        if ((typeSyntax?.IsNullable() ?? false) || (typeSymbol?.IsNullable() ?? false))
        {
            textWriter.WriteLine($"if ({variableName} is null)");
            textWriter.WriteLine("{");
            textWriter.Indent++;
            textWriter.WriteLine("builder.Append(\"-1\");");
            textWriter.Indent--;
            textWriter.WriteLine("}");
            textWriter.WriteLine("else");
        }
        textWriter.WriteLine("{");
        textWriter.Indent++;
        textWriter.WriteLine($"builder.Append({variableName} ? '1' : '0');");
        textWriter.Indent--;
        textWriter.WriteLine("}");
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol, bool nullable)
    {
        textWriter.WriteLine($"{Constants.HelperClass}.ParseBool(in stringEnumerator, {nullable.ToString().ToLower()});");
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        textWriter.WriteLine(@"
public static Result<bool?> ParseBool(in PacketStringEnumerator stringEnumerator, bool nullable)
{{
    var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
    if (!tokenResult.IsSuccess)
    {{
        return Result<bool?>.FromError(tokenResult);
    }}

    var token = packetToken.Token;
    if (nullable)
    {{
        if (token.Length == 2 && token.StartsWith(""-1""))
        {{
            return Result<bool?>.FromSuccess(null);
        }}
    }}

    return token[0] == '1' ? true : false;
}}
");
    }
}
