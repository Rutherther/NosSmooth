//
//  SourceGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.AttributeGenerators;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;
using NosSmooth.PacketSerializersGenerator.InlineConverterGenerators;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Generates IInlineGenerator for packets that are marked with NosSmooth.Packets.Attributes.GenerateSerializerAttribute.
/// </summary>
/// <remarks>
/// The packets to create serializer for have to be records that specify PacketIndices in the constructor.
/// </remarks>
[Generator]
public class SourceGenerator : ISourceGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceGenerator"/> class.
    /// </summary>
    public SourceGenerator()
    {
        _typeConverterGenerator = new List<IInlineConverterGenerator>
        (
            new IInlineConverterGenerator[]
            {
                new StringInlineConverterGenerator(),
                new BasicInlineConverterGenerator(),
                new EnumInlineConverterGenerator(),
                new BoolInlineConverterGenerator(),
            }
        );
        var inlineTypeConverter = new InlineTypeConverterGenerator(_typeConverterGenerator);
        _typeConverterGenerator.Add(new ListInlineConverterGenerator(inlineTypeConverter));

        _generators = new List<IParameterGenerator>
        (
            new IParameterGenerator[]
            {
                new PacketIndexAttributeGenerator(inlineTypeConverter),
                new PacketGreedyIndexAttributeGenerator(inlineTypeConverter),
                new PacketListIndexAttributeGenerator(inlineTypeConverter),
                new PacketContextListAttributeGenerator(inlineTypeConverter),
                new PacketConditionalIndexAttributeGenerator(inlineTypeConverter),
                new PacketConditionalListIndexAttributeGenerator(inlineTypeConverter),
            }
        );
    }

    private readonly List<IParameterGenerator> _generators;
    private readonly List<IInlineConverterGenerator> _typeConverterGenerator;

    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        // SpinWait.SpinUntil(() => Debugger.IsAttached);
    }

    private IEnumerable<RecordDeclarationSyntax> GetPacketRecords(Compilation compilation, SyntaxTree tree)
    {
        var semanticModel = compilation.GetSemanticModel(tree);
        var root = tree.GetRoot();

        return root
            .DescendantNodes()
            .OfType<RecordDeclarationSyntax>()
            .Where
            (
                x => x.AttributeLists.Any
                    (y => y.ContainsAttribute(semanticModel, Constants.GenerateSourceAttributeClass))
            );
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        var packetRecords = context.Compilation.SyntaxTrees
            .SelectMany(x => GetPacketRecords(context.Compilation, x));

        foreach (var packetRecord in packetRecords)
        {
            if (packetRecord is not null)
            {
                using var stringWriter = new StringWriter();
                using var writer = new IndentedTextWriter(stringWriter, "    ");
                var generatedResult = GeneratePacketSerializer(writer, context.Compilation, packetRecord);
                if (generatedResult is not null)
                {
                    if (generatedResult is DiagnosticError diagnosticError)
                    {
                        context.ReportDiagnostic
                        (
                            Diagnostic.Create
                            (
                                new DiagnosticDescriptor
                                (
                                    diagnosticError.Id,
                                    diagnosticError.Title,
                                    diagnosticError.MessageFormat,
                                    "Serialization",
                                    DiagnosticSeverity.Error,
                                    true
                                ),
                                Location.Create(diagnosticError.Tree, diagnosticError.Span),
                                diagnosticError.Parameters.ToArray()
                            )
                        );
                    }
                    else if (generatedResult is not null)
                    {
                        throw new Exception(generatedResult.Message);
                    }

                    continue;
                }

                context.AddSource
                (
                    $"{packetRecord.GetPrefix()}.{packetRecord.Identifier.NormalizeWhitespace().ToFullString()}Converter.g.cs",
                    stringWriter.GetStringBuilder().ToString()
                );
            }
        }

        var helperClass = GenerateHelperMethods();
        context.AddSource
        (
            $"HelperClass.g.cs",
            helperClass
        );
    }

    private string GenerateHelperMethods()
    {
        using var stringWriter = new StringWriter();
        using var writer = new IndentedTextWriter(stringWriter, "    ");
        writer.WriteMultiline(@"// <auto-generated/>
#nullable enable
#pragma warning disable 1591

using System.Collections;
using System.Collections.Generic;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using Remora.Results;

namespace NosSmooth.Generated;

internal static class HelperClass
{
");
        foreach (var inlineHelperGenerator in _typeConverterGenerator)
        {
            inlineHelperGenerator.GenerateHelperMethods(writer);
        }
        writer.WriteLine("}");
        return stringWriter.GetStringBuilder().ToString();
    }

    private IError? GeneratePacketSerializer
        (IndentedTextWriter textWriter, Compilation compilation, RecordDeclarationSyntax packetClass)
    {
        var semanticModel = compilation.GetSemanticModel(packetClass.SyntaxTree);

        var name = packetClass.Identifier.NormalizeWhitespace().ToFullString();
        var @namespace = packetClass.GetPrefix();

        var constructor = (ParameterListSyntax?)packetClass.ChildNodes()
            .FirstOrDefault(x => x.IsKind(SyntaxKind.ParameterList));

        if (constructor is null)
        {
            return new DiagnosticError
            (
                "SGConst",
                "Packet without constructor",
                "The packet class {0} does not have any constructors to use for packet serializer.",
                packetClass.SyntaxTree,
                packetClass.FullSpan,
                new List<object?>
                (
                    new[]
                    {
                        packetClass.Identifier.NormalizeWhitespace().ToFullString()
                    }
                )
            );
        }

        var parameters = constructor.Parameters;
        var orderedParameters = new List<ParameterInfo>();
        int constructorIndex = 0;
        foreach (var parameter in parameters)
        {
            var createError = CreateParameterInfo
            (
                packetClass,
                parameter,
                semanticModel,
                constructorIndex,
                out var parameterInfo
            );

            if (createError is not null)
            {
                return createError;
            }

            if (parameterInfo is not null)
            {
                orderedParameters.Add(parameterInfo);
            }

            constructorIndex++;
        }

        orderedParameters = orderedParameters.OrderBy(x => x.PacketIndex).ToList();
        var generatorAttribute = packetClass.AttributeLists.Where
                (x => x.ContainsAttribute(semanticModel, Constants.GenerateSourceAttributeClass))
            .Select
            (
                x => x.Attributes.First
                    (x => semanticModel.GetTypeInfo(x).Type!.ToString() == Constants.GenerateSourceAttributeClass)
            )
            .First();

        var packetInfo = new PacketInfo
        (
            compilation,
            CreateAttributeInfo(generatorAttribute, semanticModel),
            packetClass,
            semanticModel,
            new Parameters(orderedParameters),
            @namespace,
            name
        );

        var generator = new PacketConverterGenerator(packetInfo, _generators);
        var generatorError = generator.Generate(textWriter);

        return generatorError;
    }

    private IError? CreateParameterInfo
    (
        RecordDeclarationSyntax packet,
        ParameterSyntax parameter,
        SemanticModel semanticModel,
        int constructorIndex,
        out ParameterInfo? parameterInfo
    )
    {
        var name = packet.Identifier.NormalizeWhitespace().ToFullString();

        parameterInfo = null;
        var attributes = parameter.AttributeLists
            .Where(x => x.ContainsAttribute(semanticModel, Constants.PacketAttributesClassRegex))
            .SelectMany
            (
                x
                    => x.Attributes.Where
                    (
                        y => Regex.IsMatch
                            (semanticModel.GetTypeInfo(y).Type?.ToString()!, Constants.PacketAttributesClassRegex)
                    )
            )
            .ToList();

        if (attributes.Count == 0)
        {
            return new DiagnosticError
            (
                "SGAttr",
                "Packet constructor parameter without packet attribute",
                "Could not find PacketIndexAttribute on {0} parameter in class {1}. Parameters without PacketIndexAttribute aren't allowed.",
                parameter.SyntaxTree,
                parameter.FullSpan,
                new List<object?>
                (
                    new[]
                    {
                        parameter.Identifier.NormalizeWhitespace().ToFullString(),
                        name
                    }
                )
            );
        }

        var attribute = attributes.First();
        var index = ushort.Parse(attribute.ArgumentList!.Arguments[0].GetValue(semanticModel)!.ToString());

        List<AttributeInfo> attributeInfos = attributes
            .Select(x => CreateAttributeInfo(x, semanticModel))
            .ToList();

        parameterInfo = new ParameterInfo
        (
            parameter,
            semanticModel.GetTypeInfo(parameter.Type!).Type!,
            parameter.Type is NullableTypeSyntax,
            attributeInfos,
            parameter.Identifier.NormalizeWhitespace().ToFullString(),
            constructorIndex,
            index
        );
        return null;
    }

    private AttributeInfo CreateAttributeInfo(AttributeSyntax attribute, SemanticModel semanticModel)
    {
        var namedArguments = new Dictionary<string, AttributeArgumentInfo>();
        var arguments = new List<AttributeArgumentInfo>();

        foreach (var argument in attribute.ArgumentList!.Arguments)
        {
            var argumentName = argument.NameEquals?.Name.Identifier.NormalizeWhitespace().ToFullString();
            var value = argument.GetValue(semanticModel);
            bool isArray = argument.Expression is ArrayCreationExpressionSyntax;

            if (argumentName is not null)
            {
                namedArguments.Add
                    (argumentName, new AttributeArgumentInfo(argument, isArray, value, argument.ToString()));
            }
            else
            {
                arguments.Add(new AttributeArgumentInfo(argument, isArray, value, argument.ToString()));
            }
        }

        return new AttributeInfo
        (
            attribute,
            semanticModel.GetTypeInfo(attribute).Type?.ToString()!,
            arguments,
            namedArguments
        );
    }
}