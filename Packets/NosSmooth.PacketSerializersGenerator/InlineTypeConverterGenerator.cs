//
//  InlineTypeConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;
using NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Generates serializer or deserializer for a parameter of specified type.
/// </summary>
public class InlineTypeConverterGenerator
{
    private readonly IReadOnlyList<IInlineConverterGenerator> _typeGenerators;
    private readonly FallbackInlineConverterGenerator _fallbackInlineConverterGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineTypeConverterGenerator"/> class.
    /// </summary>
    /// <param name="typeGenerators">The type generators.</param>
    public InlineTypeConverterGenerator(IReadOnlyList<IInlineConverterGenerator> typeGenerators)
    {
        _typeGenerators = typeGenerators;
        _fallbackInlineConverterGenerator = new FallbackInlineConverterGenerator();

    }

    /// <summary>
    /// Generates deserialize call.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    /// <param name="packet">The packet.</param>
    /// <param name="nullable">Whether the parameter is nullable.</param>
    /// <returns>An error, if any.</returns>
    public IError? CallDeserialize(IndentedTextWriter textWriter, PacketInfo packet, bool nullable)
    {
        var parameter = packet.Parameters.Current;
        var shouldGenerateInline = packet.GenerateAttribute.GetIndexedValue<bool>(0);
        if (shouldGenerateInline)
        {
            foreach (var generator in _typeGenerators)
            {
                if (generator.ShouldHandle(parameter.Parameter.Type, parameter.Type))
                {
                    return generator.CallDeserialize(textWriter, parameter.Parameter.Type, parameter.Type, nullable);
                }
            }
        }

        return _fallbackInlineConverterGenerator.CallDeserialize(textWriter, parameter.Parameter.Type, parameter.Type, nullable);
    }

    /// <summary>
    /// Generates deserialize call.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    /// <param name="typeSyntax">The type syntax.</param>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="nullable">Whether the parameter is nullable.</param>
    /// <returns>An error, if any.</returns>
    public IError? CallDeserialize
    (
        IndentedTextWriter textWriter,
        TypeSyntax? typeSyntax,
        ITypeSymbol? typeSymbol,
        bool nullable
    )
    {
        foreach (var generator in _typeGenerators)
        {
            if (generator.ShouldHandle(typeSyntax, typeSymbol))
            {
                return generator.CallDeserialize(textWriter, typeSyntax, typeSymbol, nullable);
            }
        }

        return _fallbackInlineConverterGenerator.CallDeserialize(textWriter, typeSyntax, typeSymbol, nullable);
    }

    /// <summary>
    /// Generates serialize code.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    /// <param name="packet">The packet.</param>
    /// <returns>An error, if any.</returns>
    public IError? Serialize(IndentedTextWriter textWriter, PacketInfo packet)
    {
        var parameter = packet.Parameters.Current;
        var variableName = "obj." + parameter.Name;
        var shouldGenerateInline = packet.GenerateAttribute.GetIndexedValue<bool>(0);
        if (shouldGenerateInline)
        {
            foreach (var generator in _typeGenerators)
            {
                if (generator.ShouldHandle(parameter.Parameter.Type, parameter.Type))
                {
                    return generator.GenerateSerializerPart
                        (textWriter, variableName, parameter.Parameter.Type, parameter.Type);
                }
            }
        }

        return _fallbackInlineConverterGenerator.GenerateSerializerPart
            (textWriter, variableName, parameter.Parameter.Type, parameter.Type);
    }

    /// <summary>
    /// Generates serialize code.
    /// </summary>
    /// <param name="textWriter">The text writer.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="typeSyntax">The type syntax.</param>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>An error, if any.</returns>
    public IError? Serialize
    (
        IndentedTextWriter textWriter,
        string variableName,
        TypeSyntax? typeSyntax,
        ITypeSymbol? typeSymbol
    )
    {
        foreach (var generator in _typeGenerators)
        {
            if (generator.ShouldHandle(typeSyntax, typeSymbol))
            {
                return generator.GenerateSerializerPart
                    (textWriter, variableName, typeSyntax, typeSymbol);
            }
        }

        return _fallbackInlineConverterGenerator.GenerateSerializerPart
            (textWriter, variableName, typeSyntax, typeSymbol);
    }
}