//
//  BasicInlineConverterGenerator.cs
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
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
        => HandleTypes.Contains(typeSyntax?.ToString().TrimEnd('?')) || HandleTypes.Contains(typeSymbol?.ToString().TrimEnd('?'));

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, string variableName, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
    {
        if ((typeSyntax is not null && typeSyntax.IsNullable()) || (typeSymbol is not null && (typeSymbol.IsNullable() ?? false)))
        {
            textWriter.WriteLine($"if ({variableName} is null)");
            textWriter.WriteLine("{");
            textWriter.WriteLine("builder.Append(\"-1\");");
            textWriter.WriteLine("}");
            textWriter.WriteLine("else");
        }
        textWriter.WriteLine("{");
        textWriter.WriteLine
            ($"builder.Append(({(typeSymbol?.ToString() ?? typeSyntax!.ToString()).TrimEnd('?')}){variableName});");
        textWriter.WriteLine("}");
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol, bool nullable)
    {
        var type = typeSyntax is not null
            ? typeSyntax.ToString().TrimEnd('?')
            : typeSymbol?.ToString().TrimEnd('?');
        if (type is null)
        {
            throw new Exception("TypeSyntax or TypeSymbol has to be non null.");
        }

        textWriter.WriteLine($"{Constants.HelperClass}.ParseBasic{type}(typeConverter, ref stringEnumerator, {nullable.ToString().ToLower()});");
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        foreach (var type in HandleTypes)
        {
            textWriter.WriteMultiline($@"
public static Result<{type}?> ParseBasic{type}(IStringConverter typeConverter, ref PacketStringEnumerator stringEnumerator, bool nullable)
{{
    var tokenResult = stringEnumerator.GetNextToken(out var packetToken);
    if (!tokenResult.IsSuccess)
    {{
        return Result<{type}?>.FromError(tokenResult);
    }}

    var token = packetToken.Token;
    if (nullable)
    {{
        if (token.Length == 2 && token.StartsWith(""-1""))
        {{
            return Result<{type}?>.FromSuccess(null);
        }}
    }}

    if (!{type}.TryParse(token, out var val))
    {{
        return new CouldNotConvertError(typeConverter, token.ToString(), ""Could not convert as {type} in inline converter"");
    }}

    return val;
}}
");
        }
    }
}
