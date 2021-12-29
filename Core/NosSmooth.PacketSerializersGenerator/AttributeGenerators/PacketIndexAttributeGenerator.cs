//
//  PacketIndexAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Errors;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators
{
    /// <inheritdoc />
    public class PacketIndexAttributeGenerator : IParameterGenerator
    {
        /// <inheritdoc />
        public bool ShouldHandle(AttributeSyntax attribute)
            => attribute.Name.NormalizeWhitespace().ToFullString() == "PacketIndex";

        /// <inheritdoc />
        public IError? GenerateSerializerPart(IndentedTextWriter textWriter, RecordDeclarationSyntax recordDeclarationSyntax, ParameterInfo parameterInfo)
        {
            textWriter.WriteLine($@"
var {parameterInfo.Name}Result = _typeConverterRepository.Serialize(obj.{parameterInfo.Name}, builder);
if (!{parameterInfo.Name}Result.IsSuccess)
{{
    return Result.FromError(new PacketParameterSerializerError(this, ""{parameterInfo.Name}"", {parameterInfo.Name}Result), {parameterInfo.Name}Result);
}}
");

            return null;
        }

        /// <inheritdoc />
        public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, RecordDeclarationSyntax recordDeclarationSyntax, ParameterInfo parameterInfo)
        {
            bool pushedLevel = false;

            if (parameterInfo.NamedAttributeArguments.ContainsKey("AfterSeparator") && parameterInfo.NamedAttributeArguments["AfterSeparator"] is not null)
            {
                textWriter.WriteLine($"stringEnumerator.SetAfterSeparatorOnce('{parameterInfo.NamedAttributeArguments["AfterSeparator"]}');");
            }

            if (parameterInfo.NamedAttributeArguments.ContainsKey("InnerSeparator") && parameterInfo.NamedAttributeArguments["InnerSeparator"] is not null)
            {
                pushedLevel = true;
                textWriter.WriteLine($"stringEnumerator.PushLevel('{parameterInfo.NamedAttributeArguments["InnerSeparator"]}');");
            }

            var semanticModel = parameterInfo.Compilation.GetSemanticModel(recordDeclarationSyntax.SyntaxTree);
            var type = semanticModel.GetTypeInfo(parameterInfo.Parameter.Type!).Type;
            string last = parameterInfo.IsLast ? "true" : "false";
            textWriter.WriteLine($@"
var {parameterInfo.Name}Result = _typeConverterRepository.Deserialize<{type!.ToString()}>(stringEnumerator);
var {parameterInfo.Name}Error = CheckDeserializationResult({parameterInfo.Name}Result, ""{parameterInfo.Name}"", stringEnumerator, {last});
if ({parameterInfo.Name}Error is not null)
{{
        return Result<{recordDeclarationSyntax.Identifier.NormalizeWhitespace().ToFullString()}?>.FromError({parameterInfo.Name}Error, {parameterInfo.Name}Result);
}}

var {parameterInfo.Name} = {parameterInfo.Name}Result.Entity;
");
            if (pushedLevel)
            {
                textWriter.WriteLine("stringEnumerator.PopLevel();");
            }

            return null;
        }
    }
}