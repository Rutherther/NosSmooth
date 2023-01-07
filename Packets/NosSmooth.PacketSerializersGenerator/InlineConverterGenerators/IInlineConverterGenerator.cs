//
//  IInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;

namespace NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

/// <summary>
/// Generates inline type converters.
/// </summary>
public interface IInlineConverterGenerator
{
    /// <summary>
    /// Whether the given parameter should be handled by this.
    /// </summary>
    /// <param name="typeSyntax">The type syntax.</param>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>Whether to handle.</returns>
    public bool ShouldHandle(TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol);

    /// <summary>
    /// Generate the serializer part.
    /// </summary>
    /// <param name="textWriter">The text writer to write to.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="typeSyntax">The type syntax.</param>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>An error, if any.</returns>
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, string variableName, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol);

    /// <summary>
    /// Generate the deserializer part.
    /// </summary>
    /// <param name="textWriter">The text writer to write to.</param>
    /// <param name="typeSyntax">The type syntax.</param>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="nullable">Whether the parameter is nullable.</param>
    /// <returns>An error, if any.</returns>
    public IError? CallDeserialize(IndentedTextWriter textWriter, TypeSyntax? typeSyntax, ITypeSymbol? typeSymbol, bool nullable);

    /// <summary>
    /// Generate helper methods to HelperClass.
    /// </summary>
    /// <remarks>
    /// These will be added to class <see cref="Constants.HelperClass"/>.
    /// This will be called after calling <see cref="CallDeserialize"/> and
    /// <see cref="GenerateSerializerPart"/> for all packets and parameters.
    /// The converter can group information it needs for the generations that way.
    /// </remarks>
    /// <param name="textWriter">The text writer to append full methods to.</param>
    public void GenerateHelperMethods(IndentedTextWriter textWriter);
}