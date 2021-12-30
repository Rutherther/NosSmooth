//
//  PacketListIndexAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Errors;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketListIndexAttributeGenerator : IParameterGenerator
{
    /// <inheritdoc />
    public bool ShouldHandle(AttributeSyntax attribute)
        => attribute.Name.NormalizeWhitespace().ToFullString() == "PacketListIndex";

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, RecordDeclarationSyntax recordDeclarationSyntax, ParameterInfo parameterInfo)
    {
        if (parameterInfo.NamedAttributeArguments.ContainsKey("AfterSeparator") && parameterInfo.NamedAttributeArguments["AfterSeparator"] is not null)
        {
            textWriter.WriteLine($"builder.SetAfterSeparatorOnce('{parameterInfo.NamedAttributeArguments["AfterSeparator"]}');");
        }

        var listSeparator = '|';
        if (parameterInfo.NamedAttributeArguments.ContainsKey("ListSeparator") && parameterInfo.NamedAttributeArguments["ListSeparator"] is not null)
        {
            listSeparator = parameterInfo.NamedAttributeArguments["ListSeparator"]!.ToString()[0];
        }

        textWriter.WriteLine($"builder.PushLevel('{listSeparator}');");

        var innerSeparator = '.';
        if (parameterInfo.NamedAttributeArguments.ContainsKey("InnerSeparator") && parameterInfo.NamedAttributeArguments["InnerSeparator"] is not null)
        {
            innerSeparator = parameterInfo.NamedAttributeArguments["InnerSeparator"]!.ToString()[0];
        }

        textWriter.WriteLine($"builder.PrepareLevel('{innerSeparator}');");

        textWriter.WriteLine($@"
var {parameterInfo.Name}Result = _typeConverterRepository.Serialize(obj.{parameterInfo.Name}, builder);
if (!{parameterInfo.Name}Result.IsSuccess)
{{
    return Result.FromError(new PacketParameterSerializerError(this, ""{parameterInfo.Name}"", {parameterInfo.Name}Result), {parameterInfo.Name}Result);
}}

builder.RemovePreparedLevel();
builder.PopLevel();
");

        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, RecordDeclarationSyntax recordDeclarationSyntax, ParameterInfo parameterInfo)
    {
        bool nullable = parameterInfo.Parameter.Type is NullableTypeSyntax;

        if (parameterInfo.NamedAttributeArguments.ContainsKey("AfterSeparator") && parameterInfo.NamedAttributeArguments["AfterSeparator"] is not null)
        {
            textWriter.WriteLine($"stringEnumerator.SetAfterSeparatorOnce('{parameterInfo.NamedAttributeArguments["AfterSeparator"]}');");
        }

        var listSeparator = '|';
        if (parameterInfo.NamedAttributeArguments.ContainsKey("ListSeparator") && parameterInfo.NamedAttributeArguments["ListSeparator"] is not null)
        {
            listSeparator = parameterInfo.NamedAttributeArguments["ListSeparator"]!.ToString()[0];
        }

        textWriter.WriteLine($"stringEnumerator.PushLevel('{listSeparator}');");

        var innerSeparator = '.';
        if (parameterInfo.NamedAttributeArguments.ContainsKey("InnerSeparator") && parameterInfo.NamedAttributeArguments["InnerSeparator"] is not null)
        {
            innerSeparator = parameterInfo.NamedAttributeArguments["InnerSeparator"]!.ToString()[0];
        }

        var maxTokens = "null";
        if (parameterInfo.NamedAttributeArguments.ContainsKey("Length") && parameterInfo.NamedAttributeArguments["Length"] is not null)
        {
            maxTokens = parameterInfo.NamedAttributeArguments["Length"]!.ToString();
        }

        textWriter.WriteLine($"stringEnumerator.PrepareLevel('{innerSeparator}', {maxTokens ?? "null"});");

        var semanticModel = parameterInfo.Compilation.GetSemanticModel(recordDeclarationSyntax.SyntaxTree);
        var type = semanticModel.GetTypeInfo(parameterInfo.Parameter.Type!).Type;
        string last = parameterInfo.IsLast ? "true" : "false";
        textWriter.WriteLine($@"
var {parameterInfo.Name}Result = _typeConverterRepository.Deserialize<{type!.ToString()}{(nullable ? string.Empty : "?")}>(stringEnumerator);
var {parameterInfo.Name}Error = CheckDeserializationResult({parameterInfo.Name}Result, ""{parameterInfo.Name}"", stringEnumerator, {last});
if ({parameterInfo.Name}Error is not null)
{{
        return Result<{recordDeclarationSyntax.Identifier.NormalizeWhitespace().ToFullString()}?>.FromError({parameterInfo.Name}Error, {parameterInfo.Name}Result);
}}


stringEnumerator.RemovePreparedLevel();
stringEnumerator.PopLevel();
");
        if (!nullable)
        {
            textWriter.WriteLine($@"
if ({parameterInfo.Name}Result.Entity is null) {{
  return new PacketParameterSerializerError(this, ""{parameterInfo.Name}"", {parameterInfo.Name}Result, ""The converter has returned null even though it was not expected."");
}}
");
        }

        textWriter.WriteLine($"var {parameterInfo.Name} = ({type.ToString()}){parameterInfo.Name}Result.Entity;");

        return null;
    }
}