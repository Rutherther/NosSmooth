//
//  PacketIndexAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc />
public class PacketIndexAttributeGenerator : IParameterGenerator
{
    private readonly InlineTypeConverterGenerator _inlineTypeConverterGenerators;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketIndexAttributeGenerator"/> class.
    /// </summary>
    /// <param name="inlineTypeConverterGenerators">The generator for types.</param>
    public PacketIndexAttributeGenerator(InlineTypeConverterGenerator inlineTypeConverterGenerators)
    {
        _inlineTypeConverterGenerators = inlineTypeConverterGenerators;
    }

    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketIndexAttributeFullName
        => "NosSmooth.PacketSerializer.Abstractions.Attributes.PacketIndexAttribute";

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Attributes.Any(x => x.FullName == PacketIndexAttributeFullName);

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
        bool pushedLevel = false;
        var generator = new ConverterSerializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First();

        if (parameter.IsOptional())
        {
            textWriter.WriteLine($"if (obj.{parameter.GetVariableName()} is not null)");
            textWriter.WriteLine("{");
            textWriter.Indent++;
        }

        // register after separator
        var afterSeparator = attribute.GetNamedValue<char?>("AfterSeparator", null);
        if (afterSeparator is not null)
        {
            generator.SetAfterSeparatorOnce((char)afterSeparator);
        }

        // push inner separator level
        var innerSeparator = attribute.GetNamedValue<char?>("InnerSeparator", null);
        if (innerSeparator is not null)
        {
            generator.PushLevel((char)innerSeparator);
            pushedLevel = true;
        }

        // serialize, check the error.
        _inlineTypeConverterGenerators.Serialize(textWriter, packetInfo);

        // pop inner separator level
        if (pushedLevel)
        {
            generator.PopLevel();
        }

        var allowMultipleSeparators = attribute.GetNamedValue("AllowMultipleSeparators", false);
        var multipleSeparatorsCount = attribute.GetNamedValue<int>("MultipleSeparatorsCount", 1);

        if (allowMultipleSeparators)
        {
            for (var i = 0; i < multipleSeparatorsCount; i++)
            {
                textWriter.WriteLine("builder.Append(string.Empty);");
            }
        }

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
        bool pushedLevel = false;
        var generator = new ConverterDeserializationGenerator(textWriter);
        var parameter = packetInfo.Parameters.Current;
        var attribute = parameter.Attributes.First();

        generator.DeclareLocalVariable(parameter);

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

        var innerSeparator = attribute.GetNamedValue<char?>("InnerSeparator", null);
        if (innerSeparator is not null)
        {
            generator.PushLevel((char)innerSeparator);
            pushedLevel = true;
        }

        generator.DeserializeAndCheck(parameter, packetInfo, _inlineTypeConverterGenerators);
        if (!parameter.Nullable)
        {
            generator.CheckNullError
                (parameter.GetNullableVariableName(), parameter.GetResultVariableName(), parameter.Name);
        }

        generator.AssignLocalVariable(parameter, false);
        if (pushedLevel)
        {
            generator.ReadToLastToken();
            generator.PopLevel();
        }

        var allowMultipleSeparators = attribute.GetNamedValue<bool>("AllowMultipleSeparators", false);

        if (allowMultipleSeparators)
        {
            textWriter.WriteLine("while (stringEnumerator.IsOnSeparator())");
            textWriter.WriteLine("{");
            textWriter.WriteLine("stringEnumerator.GetNextToken(out _);");
            textWriter.WriteLine("}");
        }

        // end is last token if body
        if (parameter.IsOptional())
        {
            generator.EndOptionalCheck(parameter);
        }

        return null;
    }
}