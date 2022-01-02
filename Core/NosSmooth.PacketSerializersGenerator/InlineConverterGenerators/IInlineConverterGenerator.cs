//
//  IInlineConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
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
    /// <param name="parameter">The parameter.</param>
    /// <returns>Whethet to handle.</returns>
    public bool ShouldHandle(ParameterInfo parameter);

    /// <summary>
    /// Generate the serializer part.
    /// </summary>
    /// <param name="textWriter">The text writer to write to.</param>
    /// <param name="packet">The packet.</param>
    /// <returns>An error, if any.</returns>
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packet);

    /// <summary>
    /// Generate the deserializer part.
    /// </summary>
    /// <param name="textWriter">The text writer to write to.</param>
    /// <param name="packet">The packet.</param>
    /// <returns>An error, if any.</returns>
    public IError? CallDeserialize(IndentedTextWriter textWriter, PacketInfo packet);

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