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
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;
using ParameterInfo = NosSmooth.PacketSerializersGenerator.Data.ParameterInfo;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketListIndexAttributeGenerator : IParameterGenerator
{
    private readonly InlineTypeConverterGenerator _inlineTypeConverterGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketListIndexAttributeGenerator"/> class.
    /// </summary>
    /// <param name="inlineTypeConverterGenerators">The generator for types.</param>
    public PacketListIndexAttributeGenerator(InlineTypeConverterGenerator inlineTypeConverterGenerators)
    {
        _inlineTypeConverterGenerators = inlineTypeConverterGenerators;
    }

    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketListIndexAttributeFullName
        => "NosSmooth.PacketSerializer.Abstractions.Attributes.PacketListIndexAttribute";

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

        generator.DeclareLocalVariable(parameter);

        // add optional if
        if (parameter.IsOptional())
        {
            generator.StartOptionalCheck(parameter, packetInfo.Name);
        }

        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        var listSeparator = attribute.GetNamedValue<char>("ListSeparator", '|');
        var length = attribute.GetNamedValue<int>("Length", 0);
        generator.PushLevel(listSeparator, length != 0 ? (uint?)length : (uint?)null);

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

        generator.AssignLocalVariable(parameter, false);

        if (!packetInfo.Parameters.IsLast)
        {
            generator.ValidateNotLast(parameter.Name);
        }

        // end is last token if body
        if (parameter.IsOptional())
        {
            generator.EndOptionalCheck(parameter);
        }

        return null;
    }
}