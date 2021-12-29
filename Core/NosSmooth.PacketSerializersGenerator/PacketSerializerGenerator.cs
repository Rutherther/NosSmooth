//
//  PacketSerializerGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.AttributeGenerators;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Generates ITypeGenerator for packets that are marked with NosSmooth.Packets.Attributes.GenerateSerializerAttribute.
/// </summary>
/// <remarks>
/// The packets to create serializer for have to be records that specify PacketIndices in the constructor.
/// </remarks>
[Generator]
public class PacketSerializerGenerator : ISourceGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketSerializerGenerator"/> class.
    /// </summary>
    public PacketSerializerGenerator()
    {
        _generators = new List<IParameterGenerator>(new[] { new PacketIndexAttributeGenerator() });
    }

    private readonly List<IParameterGenerator> _generators;

    /// <inheritdoc />
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new PacketClassReceiver());
    }

    /// <inheritdoc />
    public void Execute(GeneratorExecutionContext context)
    {
        var syntaxReceiver = (PacketClassReceiver)context.SyntaxReceiver!;

        foreach (var packetClass in syntaxReceiver.PacketClasses)
        {
            if (packetClass is not null)
            {
                using var stringWriter = new StringWriter();
                var writer = new IndentedTextWriter(stringWriter, "    ");
                var generatedResult = GeneratePacketSerializer(writer, context.Compilation, packetClass);
                if (generatedResult is not null)
                {
                    if (generatedResult is DiagnosticError diagnosticError)
                    {
                        context.ReportDiagnostic(Diagnostic.Create
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

                context.AddSource($"{packetClass.Identifier.NormalizeWhitespace().ToFullString()}Converter.g.cs", stringWriter.GetStringBuilder().ToString());
            }
        }
    }

    private IError? GeneratePacketSerializer(IndentedTextWriter textWriter, Compilation compilation, RecordDeclarationSyntax packetClass)
    {
        var semanticModel = compilation.GetSemanticModel(packetClass.SyntaxTree);

        var name = packetClass.Identifier.NormalizeWhitespace().ToFullString();
        var @namespace = packetClass.GetPrefix();

        var constructor = (ParameterListSyntax?)packetClass.ChildNodes()
            .FirstOrDefault(x => x.IsKind(SyntaxKind.ParameterList));

        if (constructor is null)
        {
            return new DiagnosticError(
                "SG0001",
                "Packet without constructor",
                "The packet class {0} does not have any constructors to use for packet serializer.",
                packetClass.SyntaxTree,
                packetClass.FullSpan,
                new List<object?>(new[]
                {
                    packetClass.Identifier.NormalizeWhitespace().ToFullString()
                })
            );
        }

        var parameters = constructor.Parameters;
        var orderedParameters = new List<ParameterInfo>();
        int constructorIndex = 0;
        foreach (var parameter in parameters)
        {
            var attributeLists = parameter.AttributeLists
                .Where(x => x.Attributes.Any(x => x.Name.NormalizeWhitespace().ToFullString().StartsWith("Packet"))).ToList();
            var attributes = attributeLists.FirstOrDefault()?.Attributes.Where(x => x.Name.NormalizeWhitespace().ToFullString().StartsWith("Packet"))
                .ToList();

            if (attributeLists.Count > 1 || (attributes is not null && attributes?.Count > 1))
            {
                return new DiagnosticError
                (
                    "SG0002",
                    "Packet constructor parameter with multiple packet attributes",
                    "There are multiple PacketIndexAttributes on {0} parameter in class {1}. Only one may be specified.",
                    parameter.SyntaxTree,
                    parameter.FullSpan,
                    new List<object?>(new[]
                    {
                        parameter.Identifier.NormalizeWhitespace().ToFullString(),
                        name
                    })
                );
            }
            else if (attributeLists.Count == 0 || attributes is null || attributes.Count < 1)
            {
                return new DiagnosticError(
                    "SG0003",
                    "Packet constructor parameter without packet attribute",
                    "Could not find PacketIndexAttribute on {0} parameter in class {1}. Parameters without PacketIndexAttribute aren't allowed.",
                    parameter.SyntaxTree,
                    parameter.FullSpan,
                    new List<object?>(new[]
                    {
                        parameter.Identifier.NormalizeWhitespace().ToFullString(),
                        name
                    })
                );
            }

            var attribute = attributes.First();
            var indexArg = attribute.ArgumentList!.Arguments[0];
            var indexExp = indexArg.Expression;
            var index = ushort.Parse(semanticModel.GetConstantValue(indexExp).ToString());
            var namedArguments = new Dictionary<string, object?>();
            var arguments = new List<object?>();

            foreach (var argument in attribute.ArgumentList.Arguments)
            {
                var argumentName = argument.NameEquals?.Name.Identifier.NormalizeWhitespace().ToFullString();
                var expression = argument.Expression;
                var value = semanticModel.GetConstantValue(expression).Value;

                if (argumentName is not null)
                {
                    namedArguments.Add(argumentName, value);
                }
                else
                {
                    arguments.Add(value);
                }
            }

            orderedParameters.Add(new ParameterInfo
                (
                    compilation,
                    parameter,
                    attribute,
                    arguments,
                    namedArguments,
                    parameter.Identifier.NormalizeWhitespace().ToFullString(),
                    constructorIndex,
                    index
                )
            );
            constructorIndex++;
        }

        orderedParameters = orderedParameters.OrderBy(x => x.PacketIndex).ToList();
        orderedParameters.Last().IsLast = true;

        textWriter.WriteLine(@$"// <auto-generated/>
#nullable enable
#pragma warning disable 1591

using {@namespace};
using NosSmooth.Packets.Converters;
using NosSmooth.Packets.Errors;
using NosSmooth.Packets;
using Remora.Results;

namespace {@namespace}.Generated;

public class {name}Converter : BaseTypeConverter<{name}>
{{");
        textWriter.Indent++;
        textWriter.WriteLine($@"
private readonly ITypeConverterRepository _typeConverterRepository;

public {name}Converter(ITypeConverterRepository typeConverterRepository)
{{
    _typeConverterRepository = typeConverterRepository;
}}

/// <inheritdoc />
public override Result Serialize({name}? obj, PacketStringBuilder builder)
{{
    if (obj is null)
    {{
        return new ArgumentNullError(nameof(obj));
    }}
");
        textWriter.Indent++;
        var serializerError = GenerateSerializer(textWriter, packetClass, orderedParameters);
        if (serializerError is not null)
        {
            return serializerError;
        }

        textWriter.Indent--;
        textWriter.WriteLine($@"
}}

/// <inheritdoc />
public override Result<{name}?> Deserialize(PacketStringEnumerator stringEnumerator)
{{
");

        textWriter.Indent++;
        var deserializerError = GenerateDeserializer(textWriter, packetClass, orderedParameters);
        if (deserializerError is not null)
        {
            return deserializerError;
        }
        textWriter.Indent--;

        textWriter.WriteLine($@"
    }}

private IResultError? CheckDeserializationResult<T>(Result<T> result, string property, PacketStringEnumerator stringEnumerator, bool last = false)
{{
    if (!result.IsSuccess)
    {{
        return new PacketParameterSerializerError(this, property, result);
    }}

    if (!last && (stringEnumerator.IsOnLastToken() ?? false))
    {{
        return new PacketEndNotExpectedError(this, property);
    }}

    return null;
}}
}}");
        return null;
    }

    private IError? GenerateSerializer(IndentedTextWriter textWriter, RecordDeclarationSyntax packetClass, List<ParameterInfo> parameters)
    {
        foreach (var parameter in parameters)
        {
            bool handled = false;
            foreach (var generator in _generators)
            {
                if (generator.ShouldHandle(parameter.Attribute))
                {
                    var result = generator.GenerateSerializerPart(textWriter, packetClass, parameter);
                    if (result is not null)
                    {
                        return result;
                    }

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                throw new InvalidOperationException($"Could not handle {packetClass.Identifier.NormalizeWhitespace().ToFullString()}.{parameter.Name}");
            }
        }

        textWriter.WriteLine("return Result.FromSuccess();");
        return null;
    }

    private IError? GenerateDeserializer(IndentedTextWriter textWriter, RecordDeclarationSyntax packetClass, List<ParameterInfo> parameters)
    {
        var lastIndex = parameters.FirstOrDefault()?.PacketIndex ?? 0;
        bool skipped = false;
        foreach (var parameter in parameters)
        {
            var skip = parameter.PacketIndex - lastIndex - 1;
            if (skip > 0)
            {
                if (!skipped)
                {
                    textWriter.WriteLine("Result<PacketToken> skipResult;");
                    textWriter.WriteLine("IResultError? skipError;");
                    skipped = true;
                }
                textWriter.WriteLine($@"skipResult = stringEnumerator.GetNextToken();");
                textWriter.WriteLine($@"skipError = CheckDeserializationResult(result, ""None"", stringEnumerator, false);");
                textWriter.WriteLine($@"if (skipError is not null) {{
    return Result<{packetClass.Identifier.NormalizeWhitespace().ToFullString()}>.FromError(skipError, skipResult);
}}");
            }

            bool handled = false;
            foreach (var generator in _generators)
            {
                if (generator.ShouldHandle(parameter.Attribute))
                {
                    var result = generator.GenerateDeserializerPart(textWriter, packetClass, parameter);
                    if (result is not null)
                    {
                        return result;
                    }

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                throw new InvalidOperationException($"Could not handle {packetClass.Identifier.NormalizeWhitespace().ToFullString()}.{parameter.Name}");
            }
            lastIndex = parameter.PacketIndex;
        }

        string parametersString = string.Join(", ", parameters.OrderBy(x => x.ConstructorIndex).Select(x => x.Name));
        textWriter.WriteLine($"return new {packetClass.Identifier.NormalizeWhitespace().ToFullString()}({parametersString});");

        return null;
    }
}