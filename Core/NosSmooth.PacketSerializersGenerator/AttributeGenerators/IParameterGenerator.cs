//
//  IParameterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators
{
    /// <summary>
    /// Generate serializer and deserializer method parts for the given constructor parameter.
    /// </summary>
    public interface IParameterGenerator
    {
        /// <summary>
        /// Check whether this generator should handle parameter with this attribute.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Whether to handle this parameter.</returns>
        public bool ShouldHandle(ParameterInfo parameter);

        /// <summary>
        /// Checks the given parameter, returns an error if the parameter cannot be processed.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>An error, if any.</returns>
        public IError? CheckParameter(PacketInfo packet, ParameterInfo parameter);

        /// <summary>
        /// Generate part for the Serializer method to serialize the given parameter.
        /// </summary>
        /// <param name="textWriter">The text writer to write the code to.</param>
        /// <param name="packetInfo">The packet info to generate for.</param>
        /// <returns>The generated source code.</returns>
        public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo);

        /// <summary>
        /// Generate part for the Deserializer method to deserialize the given parameter.
        /// </summary>
        /// <param name="textWriter">The text writer to write the code to.</param>
        /// <param name="packetInfo">The packet info to generate for.</param>
        /// <returns>The generated source code.</returns>
        public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo);
    }
}