//
//  IParameterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        /// <param name="attribute">The parameters attribute.</param>
        /// <returns>Whether to handle this parameter.</returns>
        public bool ShouldHandle(AttributeSyntax attribute);

        /// <summary>
        /// Generate part for the Serializer method to serialize the given parameter.
        /// </summary>
        /// <param name="textWriter">The text writer to write the code to.</param>
        /// <param name="recordDeclarationSyntax">The packet record declaration syntax.</param>
        /// <param name="parameterInfo">The parameter info to generate for.</param>
        /// <returns>The generated source code.</returns>
        public IError? GenerateSerializerPart(IndentedTextWriter textWriter, RecordDeclarationSyntax recordDeclarationSyntax, ParameterInfo parameterInfo);

        /// <summary>
        /// Generate part for the Deserializer method to deserialize the given parameter.
        /// </summary>
        /// <param name="textWriter">The text writer to write the code to.</param>
        /// <param name="recordDeclarationSyntax">The packet record declaration syntax.</param>
        /// <param name="parameterInfo">The parameter info to generate for.</param>
        /// <returns>The generated source code.</returns>
        public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, RecordDeclarationSyntax recordDeclarationSyntax, ParameterInfo parameterInfo);
    }
}