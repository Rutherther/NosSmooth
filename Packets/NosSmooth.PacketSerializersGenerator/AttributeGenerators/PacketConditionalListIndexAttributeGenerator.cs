//
//  PacketConditionalListIndexAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketConditionalListIndexAttributeGenerator : IParameterGenerator
{
    private readonly PacketListIndexAttributeGenerator _listIndexGenerator;
    private readonly InlineTypeConverterGenerator _inlineTypeConverterGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketConditionalListIndexAttributeGenerator"/> class.
    /// </summary>
    /// <param name="inlineTypeConverterGenerators">The generator for types.</param>
    public PacketConditionalListIndexAttributeGenerator(InlineTypeConverterGenerator inlineTypeConverterGenerators)
    {
        _inlineTypeConverterGenerators = inlineTypeConverterGenerators;
        _listIndexGenerator = new PacketListIndexAttributeGenerator(inlineTypeConverterGenerators)
        {
            PacketListIndexAttributeFullName = PacketConditionalListIndexAttributeFullName
        };
    }

    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketConditionalListIndexAttributeFullName
        => "NosSmooth.PacketSerializer.Abstractions.Attributes.PacketConditionalListIndexAttribute";

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Attributes.Any(x => x.FullName == PacketConditionalListIndexAttributeFullName);

    /// <inheritdoc />
    public IError? CheckParameter(PacketInfo packet, ParameterInfo parameter)
    {
        if (!parameter.Nullable)
        {
            return new DiagnosticError
            (
                "SGNull",
                "Conditional parameters must be nullable",
                "The parameter {0} in {1} has to be nullable, because it is conditional.",
                parameter.Parameter.SyntaxTree,
                parameter.Parameter.FullSpan,
                new List<object?>(new[] { parameter.Name, packet.Name })
            );
        }

        if (parameter.Attributes.Any(x => x.FullName != PacketConditionalListIndexAttributeFullName))
        {
            return new DiagnosticError
            (
                "SGAttr",
                "Packet constructor parameter with multiple packet attributes",
                "Found multiple packet attributes of multiple types on parameter {0} in {1}. PacketConditionalIndexAttribute supports multiple attributes of the same type only.",
                parameter.Parameter.SyntaxTree,
                parameter.Parameter.FullSpan,
                new List<object?>
                (
                    new[]
                    {
                        parameter.Name,
                        packet.Name
                    }
                )
            );
        }

        // Check that all attributes have the same data. (where the same data need to be)
        var firstAttribute = parameter.Attributes.First();
        if (parameter.Attributes.Any
            (
                x =>
                {
                    var index = x.GetIndexedValue<int>(0);
                    if (index != parameter.PacketIndex)
                    {
                        return true;
                    }

                    foreach (var keyValue in x.NamedAttributeArguments)
                    {
                        if (!firstAttribute.NamedAttributeArguments.ContainsKey(keyValue.Key))
                        {
                            return true;
                        }

                        if (firstAttribute.NamedAttributeArguments[keyValue.Key] != keyValue.Value)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            ))
        {
            return new DiagnosticError
            (
                "SGAttr",
                "Packet constructor parameter with multiple conflicting attribute data.",
                "Found multiple packet attributes of multiple types on parameter {0} in {1} with conflicting data. Index, IsOptional, InnerSeparator, ListSeparator, AfterSeparator all have to be the same for each attribute.",
                parameter.Parameter.SyntaxTree,
                parameter.Parameter.FullSpan,
                new List<object?>
                (
                    new[]
                    {
                        parameter.Name,
                        packet.Name
                    }
                )
            );
        }

        var mismatchedAttribute = parameter.Attributes.FirstOrDefault
        (
            x => x.IndexedAttributeArguments.Count < 4 ||
                (x.IndexedAttributeArguments[3].IsArray &&
                    (
                        (x.IndexedAttributeArguments[3].Argument.Expression as ArrayCreationExpressionSyntax)
                        ?.Initializer is null ||
                        (x.IndexedAttributeArguments[3].Argument.Expression as ArrayCreationExpressionSyntax)
                        ?.Initializer?.Expressions.Count == 0
                    )
                )
        );
        if (mismatchedAttribute is not null)
        {
            return new DiagnosticError
            (
                "SGAttr",
                "Packet conditional attribute without matching values",
                "Found PacketConditionalIndexAttribute without matchValues parameters set on {0} in {1}. At least one parameter has to be specified.",
                mismatchedAttribute.Attribute.SyntaxTree,
                mismatchedAttribute.Attribute.FullSpan,
                new List<object?>
                (
                    new[]
                    {
                        parameter.Name,
                        packet.Name
                    }
                )
            );
        }

        return ParameterChecker.CheckOptionalIsNullable(packet, parameter);
    }

    private string BuildAttributeIfPart(AttributeInfo attribute, string prefix)
    {
        var conditionParameterName = attribute.GetIndexedValue<string>(1);
        var negate = attribute.GetIndexedValue<bool>(2);
        var values = attribute.GetParamsVisualValues(3);
        if (conditionParameterName is null || values is null)
        {
            throw new ArgumentException();
        }

        var inner = string.Join
            (" || ", values.Select(x => $"{prefix}{conditionParameterName.Trim('"')} == {x?.ToString() ?? "null"}"));
        return (negate ? "!(" : string.Empty) + inner + (negate ? ")" : string.Empty);
    }

    private string BuildParameterIfStatement(ParameterInfo parameter, string prefix)
    {
        var ifInside = string.Empty;
        foreach (var attribute in parameter.Attributes)
        {
            ifInside += BuildAttributeIfPart(attribute, prefix);
        }

        return $"if ({ifInside})";
    }

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        var parameter = packetInfo.Parameters.Current;

        // begin conditional if
        textWriter.WriteLine(BuildParameterIfStatement(parameter, "obj."));
        textWriter.WriteLine("{");
        textWriter.Indent++;

        var error = _listIndexGenerator.GenerateSerializerPart(textWriter, packetInfo);
        if (error is not null)
        {
            return error;
        }

        // end conditional if
        textWriter.Indent--;
        textWriter.WriteLine("}");

        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        var generator = new ConverterDeserializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;

        generator.DeclareLocalVariable(parameter);

        // begin conditional if
        textWriter.WriteLine(BuildParameterIfStatement(parameter, string.Empty));
        textWriter.WriteLine("{");
        textWriter.Indent++;

        var error = _listIndexGenerator.GenerateDeserializerPart(textWriter, packetInfo, false);
        if (error is not null)
        {
            return error;
        }

        // end conditional if
        textWriter.Indent--;
        textWriter.WriteLine("}");
        textWriter.WriteLine("else");
        textWriter.WriteLine("{");
        textWriter.Indent++;
        textWriter.WriteLine($"{parameter.GetVariableName()} = null;");
        textWriter.Indent--;
        textWriter.WriteLine("}");

        return null;
    }
}