//
//  PacketGreedyIndexAttributeGenerator.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom.Compiler;
using NosSmooth.PacketSerializersGenerator.Data;
using NosSmooth.PacketSerializersGenerator.Errors;
using NosSmooth.PacketSerializersGenerator.Extensions;

namespace NosSmooth.PacketSerializersGenerator.AttributeGenerators;

/// <inheritdoc/>
public class PacketGreedyIndexAttributeGenerator : IParameterGenerator
{
    private PacketIndexAttributeGenerator _basicAttributeGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketGreedyIndexAttributeGenerator"/> class.
    /// </summary>
    public PacketGreedyIndexAttributeGenerator()
    {
        _basicAttributeGenerator = new PacketIndexAttributeGenerator();
    }

    /// <summary>
    /// Gets the full name of the packet index attribute.
    /// </summary>
    public static string PacketIndexAttributeFullName => "NosSmooth.Packets.Attributes.PacketGreedyIndexAttribute";

    /// <inheritdoc />
    public bool ShouldHandle(ParameterInfo parameter)
        => parameter.Attributes.Any(x => x.FullName == PacketIndexAttributeFullName);

    /// <inheritdoc />
    public IError? GenerateSerializerPart(IndentedTextWriter textWriter, PacketInfo packetInfo)
        => _basicAttributeGenerator.GenerateSerializerPart(textWriter, packetInfo);

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
            var error = generator.StartOptionalCheck(parameter, packetInfo.Name);
            if (error is not null)
            {
                return error;
            }
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

        generator.SetReadToLast(); // Greedy
        generator.DeserializeAndCheck
            ($"{packetInfo.Namespace}.{packetInfo.Name}", parameter, packetInfo.Parameters.IsLast);

        if (!parameter.Nullable)
        {
            generator.CheckNullError(parameter.GetResultVariableName(), parameter.Name);
        }

        generator.AssignLocalVariable(parameter, false);

        if (pushedLevel)
        {
            generator.ReadToLastToken();
            generator.PopLevel();
        }

        // end is last token if body
        if (parameter.IsOptional())
        {
            generator.EndOptionalCheck(parameter);
        }

        return null;
    }
}