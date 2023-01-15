//
//  PacketContextListAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketContextListAttributeGenerator : IParameterGenerator
{
    private readonly InlineTypeConverterGenerator _inlineTypeConverterGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketContextListAttributeGenerator"/> class.
    /// </summary>
    /// <param name="inlineTypeConverterGenerators">The generator for types.</param>
    public PacketContextListAttributeGenerator(InlineTypeConverterGenerator inlineTypeConverterGenerators)
    {
        _inlineTypeConverterGenerators = inlineTypeConverterGenerators;
    }

    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketListIndexAttributeFullName
        => "NosSmooth.PacketSerializer.Abstractions.Attributes.PacketContextListAttribute";

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Attributes.Any(x => x.FullName == PacketListIndexAttributeFullName);

    /// <inheritdoc />
    public IError? CheckParameter(PacketInfo packet, ParameterInfo parameter)
    {
        var error = ParameterChecker.CheckHasOneAttribute(packet, parameter);
        if (error is not null)
        {
            return error;
        }

        return ParameterChecker.CheckOptionalIsNullable(packet, parameter);
    }

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        var generator = new ConverterSerializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First(x => x.FullName == PacketListIndexAttributeFullName);

        if (parameter.IsOptional())
        {
            textWriter.WriteLine($"if (obj.{parameter.GetVariableName()} is not null)");
            textWriter.WriteLine("{");
            textWriter.Indent++;
        }

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var listSeparator = attribute.GetNamedValue<char>("ListSeparator", '|');
        generator.PushLevel(listSeparator);

        var innerSeparator = attribute.GetNamedValue<char>("InnerSeparator", '.');
        generator.PrepareLevel(innerSeparator);

        _inlineTypeConverterGenerators.Serialize(textWriter, packetInfo);
        generator.RemovePreparedLevel();
        generator.PopLevel();

        // end optional if
        if (parameter.IsOptional())
        {
            textWriter.Indent--;
            textWriter.WriteLine("}");
        }

        return null;
    }

    /// <inheritdoc />
    public IError? GenerateDeserializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
    {
        var generator = new ConverterDeserializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First(x => x.FullName == PacketListIndexAttributeFullName);

        // add optional if
        if (parameter.IsOptional())
        {
            generator.StartOptionalCheck(parameter, packetInfo.Name);
        }
        else
        {
            generator.ValidateNotLast(parameter.Name);
        }

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var listSeparator = attribute.GetNamedValue<char>("ListSeparator", '|');
        var lengthVariable = attribute.GetIndexedValue<string>(1)?.Trim('"');
        textWriter.WriteLine(@$"stringEnumerator.PushLevel('{listSeparator}', {lengthVariable});");

        var innerSeparator = attribute.GetNamedValue<char>("InnerSeparator", '.');
        generator.PrepareLevel(innerSeparator);

        generator.DeserializeAndCheck(parameter, packetInfo, _inlineTypeConverterGenerators);
        generator.RemovePreparedLevel();
        generator.PopLevel();
        if (!parameter.Nullable)
        {
            generator.CheckNullError
                (parameter.GetNullableVariableName(), parameter.GetResultVariableName(), parameter.Name);
        }

        generator.AssignLocalVariable(parameter);

        // end is last token if body
        if (parameter.IsOptional())
        {
            generator.EndOptionalCheck(parameter);
        }

        return null;
    }
}