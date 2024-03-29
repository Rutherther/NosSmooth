//
//  PacketConverterGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.AttributeGenerators;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Code generator of a packet converter.
/// </summary>
public class PacketConverterGenerator
{
    private readonly PacketInfo _packetInfo;
    private readonly IReadOnlyList<IParameterGenerator> _parameterGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketConverterGenerator"/> class.
    /// </summary>
    /// <param name="packetInfo">The packet type information.</param>
    /// <param name="parameterGenerators">The converter parameter generators.</param>
    public PacketConverterGenerator(PacketInfo packetInfo, IReadOnlyList<IParameterGenerator> parameterGenerators)
    {
        _packetInfo = packetInfo;
        _parameterGenerators = parameterGenerators;
    }

    /// <summary>
    /// Generate the converter class.
    /// </summary>
    /// <param name="textWriter">The text writer to write the class to.</param>
    /// <returns>An error, if any.</returns>
    public IError? Generate(IndentedTextWriter textWriter)
    {
        var usings = _packetInfo.PacketRecord.SyntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Select(x => x.ToString())
            .ToList();
        usings.Add($"using {_packetInfo.Namespace};");
        usings.Add("using NosSmooth.PacketSerializer.Abstractions.Errors;");
        usings.Add("using NosSmooth.PacketSerializer.Abstractions;");
        usings.Add("using Remora.Results;");

        var usingsString = string.Join("\n", usings.Distinct());
        textWriter.WriteLine
        (
            @$"// <auto-generated/>
#nullable enable
#pragma warning disable 1591

{usingsString}

namespace {_packetInfo.Namespace}.Generated;

public class {_packetInfo.Name}Converter : BaseStringConverter<{_packetInfo.Name}>
{{"
        );
        textWriter.Indent++;
        textWriter.WriteLine
        (
            $@"
private readonly IStringSerializer _stringSerializer;

public {_packetInfo.Name}Converter(IStringSerializer stringSerializer)
{{
    _stringSerializer = stringSerializer;
}}

/// <inheritdoc />
public override Result Serialize({_packetInfo.Name}? obj, ref PacketStringBuilder builder)
{{
    if (obj is null)
    {{
        return new ArgumentNullError(nameof(obj));
    }}
"
        );
        textWriter.Indent++;
        var serializerError = GenerateSerializer(textWriter);
        if (serializerError is not null)
        {
            return serializerError;
        }

        textWriter.Indent--;
        textWriter.WriteLine
        (
            $@"
}}

/// <inheritdoc />
public override Result<{_packetInfo.Name}?> Deserialize(ref PacketStringEnumerator stringEnumerator, DeserializeOptions options)
{{
    var typeConverter = this;
"
        );

        textWriter.Indent++;
        var deserializerError = GenerateDeserializer(textWriter);
        if (deserializerError is not null)
        {
            return deserializerError;
        }
        textWriter.Indent--;

        textWriter.WriteLine
        (
            $@"
    }}
}}"
        );
        return null;
    }

    private IError? GenerateSerializer(IndentedTextWriter textWriter)
    {
        _packetInfo.Parameters.CurrentIndex = 0;
        foreach (var parameter in _packetInfo.Parameters.List)
        {
            bool handled = false;
            foreach (var generator in _parameterGenerators)
            {
                if (generator.ShouldHandle(parameter))
                {
                    var checkError = generator.CheckParameter(_packetInfo, parameter);
                    if (checkError is not null)
                    {
                        return checkError;
                    }

                    var serializerError = generator.GenerateSerializerPart(textWriter, _packetInfo);
                    if (serializerError is not null)
                    {
                        return serializerError;
                    }

                    handled = true;
                    break;
                }
            }

            if (!handled)
            {
                throw new InvalidOperationException
                (
                    $"Could not handle {_packetInfo.Namespace}.{_packetInfo.Name}.{parameter.Name}"
                );
            }
            _packetInfo.Parameters.CurrentIndex++;
        }

        textWriter.WriteLine("return Result.FromSuccess();");
        return null;
    }

    private IError? GenerateDeserializer
        (IndentedTextWriter textWriter)
    {
        if (_packetInfo.Parameters.List.Count == 0)
        {
            textWriter.WriteLine
                ($"return new {_packetInfo.Name}();");
            return null;
        }

        _packetInfo.Parameters.CurrentIndex = 0;
        var lastIndex = _packetInfo.Parameters.Current.PacketIndex - 1;
        bool skipped = false;
        foreach (var parameter in _packetInfo.Parameters.List)
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
                textWriter.WriteLine($@"skipResult = stringEnumerator.GetNextToken(out _);");
                textWriter.WriteLine
                    ("skipError = CheckDeserializationResult(result, \"None\", stringEnumerator, false);");
                textWriter.WriteMultiline
                (
                    $@"if (skipError is not null) {{
    return Result<{_packetInfo.Name}?>.FromError(skipError, skipResult);
}}"
                );
            }
            else if (skip < 0)
            {
                return new DiagnosticError
                (
                    "SGInd",
                    "Same packet index",
                    "There were two parameters of the same packet index {0} on property {1} in packet {2}, that is not supported.",
                    parameter.Attributes.First().Attribute.SyntaxTree,
                    parameter.Attributes.First().Attribute.FullSpan,
                    new List<object?>
                    (
                        new[]
                        {
                            parameter.PacketIndex.ToString(),
                            parameter.Name,
                            _packetInfo.Name
                        }
                    )
                );
            }

            bool handled = false;
            foreach (var generator in _parameterGenerators)
            {
                if (generator.ShouldHandle(parameter))
                {
                    var checkError = generator.CheckParameter(_packetInfo, parameter);
                    if (checkError is not null)
                    {
                        return checkError;
                    }

                    var result = generator.GenerateDeserializerPart(textWriter, _packetInfo);
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
                throw new InvalidOperationException
                (
                    $"Could not handle {_packetInfo.Name}.{parameter.Name}"
                );
            }
            lastIndex = parameter.PacketIndex;
            _packetInfo.Parameters.CurrentIndex++;
        }

        string parametersString = string.Join(", ", _packetInfo.Parameters.List.OrderBy(x => x.ConstructorIndex).Select(x => x.GetVariableName()));
        textWriter.WriteLine
            ($"return new {_packetInfo.Name}({parametersString});");

        return null;
    }
}
