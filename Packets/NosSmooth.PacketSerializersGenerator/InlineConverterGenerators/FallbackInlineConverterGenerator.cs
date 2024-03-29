//
//  FallbackInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Errors;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <inheritdoc />
public class FallbackInlineConverterGenerator : IInlineConverterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol)
    {
        return true;
    }

    /// <inheritdoc />
    public IError? GenerateSerializerPart
    (
        IndentedTextWriter textWriter,
        string variableName,
        TypeSyntax? typeSyntax,
        ITypeSymbol? typeSymbol
    )
    {
        var resultName = $"{variableName.Replace(".", string.Empty)}Result";
        textWriter.WriteLine
        (
            $"var {resultName} = _stringSerializer.Serialize<{(typeSyntax?.ToString() ?? typeSymbol!.ToString()).TrimEnd('?')}?>({variableName}, ref builder);"
        );
        textWriter.WriteLine($"if (!{resultName}.IsSuccess)");
        textWriter.WriteLine("{");
        textWriter.Indent++;
        textWriter.WriteLine($"return Result.FromError(new PacketParameterSerializerError(this, \"{variableName}\", {resultName}), {resultName});");
        textWriter.Indent--;
        textWriter.WriteLine("}");
        return null;
    }

    /// <inheritdoc />
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol, bool nullable)
    {
        var options = nullable ? "DeserializeOptions.Nullable" : "default";
        textWriter.WriteLine
        (
            $"_stringSerializer.Deserialize<{(typeSyntax?.ToString() ?? typeSymbol!.ToString()).TrimEnd('?')}?>(ref stringEnumerator, {options});"
        );
        return null;
    }

    /// <inheritdoc />
    public void GenerateHelperMethods(IndentedTextWriter textWriter)
    {
        // ignore
    }
}
