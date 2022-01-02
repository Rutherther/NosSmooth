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
            .OfType<UsingDirectiveSyntax>();
        var usingsString = string.Join("\n", usings.Select(x => x.ToString()));
        textWriter.WriteLine
        (
            @$"// <auto-generated/>
#nullable enable
#pragma warning disable 1591

using {_packetInfo.Namespace};
using NosSmooth.Packets.Converters;
using NosSmooth.Packets.Errors;
using NosSmooth.Packets;
using Remora.Results;
{usingsString}

namespace {_packetInfo.Namespace}.Generated;

public class {_packetInfo.Name}Converter : BaseTypeConverter<{_packetInfo.Name}>
{{"
        );
        textWriter.Indent++;
        textWriter.WriteLine
        (
            $@"
private readonly ITypeConverterRepository _typeConverterRepository;

public {_packetInfo.Name}Converter(ITypeConverterRepository typeConverterRepository)
{{
    _typeConverterRepository = typeConverterRepository;
}}

/// <inheritdoc />
public override Result Serialize({_packetInfo.Name}? obj, PacketStringBuilder builder)
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
public override Result<{_packetInfo.Name}?> Deserialize(PacketStringEnumerator stringEnumerator)
{{
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
}}");
        var parametersSerializerError = GenerateParametersSerializerMethods(textWriter);
        if (parametersSerializerError is not null)
        {
            return parametersSerializerError;
        }

        var parametersDeserializerError = GenerateParametersDeserializerMethods(textWriter);
        textWriter.WriteLine("}");
        return parametersDeserializerError;
    }

    private IError? GenerateDeserializer(IndentedTextWriter textWriter)
    {
        _packetInfo.Parameters.CurrentIndex = 0;
        foreach (var parameter in _packetInfo.Parameters.List)
        {
            string isLastString = _packetInfo.Parameters.IsLast ? "true" : "false";
            textWriter.WriteLine
                ($"var {parameter.GetResultVariableName()} = Deserialize{parameter.Name}(stringEnumerator);");
            textWriter.WriteMultiline
            (
                @$"
var {parameter.GetErrorVariableName()} = CheckDeserializationResult({parameter.GetResultVariableName()}, ""{parameter.Name}"", stringEnumerator, {isLastString});
if ({parameter.GetErrorVariableName()} is not null)
{{
    return Result<{_packetInfo.Name}?>.FromError({parameter.GetErrorVariableName()}, {parameter.GetResultVariableName()});
}}
"
            );
            _packetInfo.Parameters.CurrentIndex++;
        }

        string parametersString = string.Join
            (", ", _packetInfo.Parameters.List.OrderBy(x => x.ConstructorIndex).Select(x => x.GetResultVariableName() + ".Entity"));
        textWriter.WriteLine
            ($"return new {_packetInfo.Name}({parametersString});");
        return null;
    }

    private IError? GenerateSerializer(IndentedTextWriter textWriter)
    {
        _packetInfo.Parameters.CurrentIndex = 0;
        foreach (var parameter in _packetInfo.Parameters.List)
        {
            textWriter.WriteLine
                ($"var {parameter.GetResultVariableName()} = Serialize{parameter.Name}(obj, builder);");
            textWriter.WriteMultiline
            (
                @$"
if (!{parameter.GetResultVariableName()}.IsSuccess)
{{
    return Result.FromError(new PacketParameterSerializerError(this, ""{parameter.Name}"", {parameter.GetResultVariableName()}), {parameter.GetResultVariableName()});
}}
"
            );
            _packetInfo.Parameters.CurrentIndex++;
        }
        textWriter.WriteLine("return Result.FromSuccess();");
        return null;
    }

    private IError? GenerateParametersSerializerMethods(IndentedTextWriter textWriter)
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

                    textWriter.WriteLine
                    (
                        $"public Result Serialize{parameter.Name}({_packetInfo.Name} obj, PacketStringBuilder builder)"
                    );
                    textWriter.WriteLine("{");
                    textWriter.Indent++;
                    var serializerError = generator.GenerateSerializerPart(textWriter, _packetInfo);
                    if (serializerError is not null)
                    {
                        return serializerError;
                    }
                    textWriter.Indent--;
                    textWriter.WriteLine("}");
                    textWriter.WriteLine();

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

        return null;
    }

    private IError? GenerateParametersDeserializerMethods
        (IndentedTextWriter textWriter)
    {
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
                textWriter.WriteLine($@"skipResult = stringEnumerator.GetNextToken();");
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

                    textWriter.WriteLine
                    (
                        $"public Result<{parameter.GetActualType()}> Deserialize{parameter.Name}(PacketStringEnumerator stringEnumerator)"
                    );
                    textWriter.WriteLine("{");
                    textWriter.Indent++;
                    var result = generator.GenerateDeserializerPart(textWriter, _packetInfo);
                    if (result is not null)
                    {
                        return result;
                    }
                    textWriter.Indent--;
                    textWriter.WriteLine("}");
                    textWriter.WriteLine();

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

        return null;
    }
}